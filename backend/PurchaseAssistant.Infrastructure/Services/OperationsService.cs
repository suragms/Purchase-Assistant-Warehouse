using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Operations;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using System.Text.RegularExpressions;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Infrastructure.Services;

public class OperationsService(AppDbContext db, ICurrentUserService user, IStockService stock) : IOperationsService
{
    private Guid Business => user.BusinessId ?? throw new UnauthorizedAccessException("Select a business.");
    private Guid User => user.UserId ?? throw new UnauthorizedAccessException();
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    private static readonly (ChecklistSlot Slot, string Key, string Label, int Priority)[] Defaults = [
        (ChecklistSlot.Morning, "open_check", "Opening stock check", 1),
        (ChecklistSlot.Morning, "fridge_temp", "Fridge / cold storage check", 2),
        (ChecklistSlot.Midday, "restock", "Midday restock check", 1),
        (ChecklistSlot.Midday, "barcode_scan", "Scan new deliveries", 2),
        (ChecklistSlot.Evening, "usage_log", "Log today's usage", 1),
        (ChecklistSlot.Evening, "closing_stock", "Closing stock verification", 2)];

    public async Task<List<ChecklistTemplateDto>> GetChecklistTemplatesAsync()
    {
        var rows = await db.Set<ChecklistTemplate>().AsNoTracking().Where(x => x.BusinessId == Business).OrderBy(x => x.Priority).ToListAsync();
        return rows.Count == 0 ? Defaults.Select(x => new ChecklistTemplateDto { Slot = x.Slot, Key = x.Key, Description = x.Label, Priority = x.Priority }).ToList()
            : rows.Select(x => new ChecklistTemplateDto { Slot = Enum.Parse<ChecklistSlot>(x.Slot), Key = x.Key, Description = x.Description, Priority = x.Priority, IsActive = x.IsActive }).ToList();
    }
    public async Task<ChecklistTodayDto> GetTodayChecklistAsync()
    {
        var templates = (await GetChecklistTemplatesAsync()).Where(x => x.IsActive).ToList();
        var done = await db.Set<ChecklistCompletion>().Where(x => x.BusinessId == Business && x.Date == Today && x.CompletedByUserId == User).ToListAsync();
        List<ChecklistTaskDto> For(ChecklistSlot slot) => templates.Where(x => x.Slot == slot).Select(t => {
            var c = done.FirstOrDefault(x => x.Slot == slot.ToString() && x.TaskKey == t.Key);
            return new ChecklistTaskDto { Key = t.Key, Description = t.Description, Priority = t.Priority, IsCompleted = c != null, CompletedAt = c?.CompletedAt, Notes = c?.Notes };
        }).ToList();
        var result = new ChecklistTodayDto { Date = Today.ToDateTime(TimeOnly.MinValue), Morning = For(ChecklistSlot.Morning), Midday = For(ChecklistSlot.Midday), Evening = For(ChecklistSlot.Evening), TotalTasks = templates.Count };
        result.CompletedTasks = result.Morning.Concat(result.Midday).Concat(result.Evening).Count(x => x.IsCompleted);
        result.CompletionPercentage = result.TotalTasks == 0 ? 0 : decimal.Round(100m * result.CompletedTasks / result.TotalTasks, 1);
        return result;
    }
    public async Task CompleteChecklistTaskAsync(ChecklistSlot slot, string taskKey, ChecklistCompleteDto dto)
    {
        if (!Enum.IsDefined(slot) || dto.Notes?.Length > 1000) throw new ArgumentException("Invalid checklist entry.");
        if (!(await GetChecklistTemplatesAsync()).Any(t => t.IsActive && t.Slot == slot && t.Key == taskKey)) throw new KeyNotFoundException("Task not found.");
        if (await db.Set<ChecklistCompletion>().AnyAsync(x => x.BusinessId == Business && x.Date == Today && x.CompletedByUserId == User && x.Slot == slot.ToString() && x.TaskKey == taskKey)) return;
        var completion = new ChecklistCompletion { BusinessId = Business, Date = Today, Slot = slot.ToString(), TaskKey = taskKey, CompletedByUserId = User, CompletedAt = DateTime.UtcNow, Notes = dto.Notes };
        db.Add(completion);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" }) { db.Entry(completion).State = EntityState.Detached; }
    }
    public async Task<List<ChecklistTemplateDto>> UpdateChecklistTemplatesAsync(List<ChecklistTemplateDto> templates)
    {
        if (user.Role is not ("Owner" or "Admin" or "Manager" or "SuperAdmin")) throw new UnauthorizedAccessException();
        if (templates == null || templates.Count is < 1 or > 100 || templates.Any(x => x == null || !Enum.IsDefined(x.Slot) || string.IsNullOrWhiteSpace(x.Description) || x.Description.Length > 255 || x.Key == null || !Regex.IsMatch(x.Key, "^[a-z0-9_]{1,64}$"))
            || templates.Select(x => (x.Slot, x.Key)).Distinct().Count() != templates.Count) throw new ArgumentException("Use unique task keys and descriptions, with 1 to 100 tasks.");
        var existing = await db.Set<ChecklistTemplate>().Where(x => x.BusinessId == Business).ToListAsync();
        foreach (var row in existing.Where(x => !templates.Any(t => t.Key == x.Key && t.Slot.ToString() == x.Slot))) db.Remove(row);
        foreach (var t in templates)
        {
            var row = existing.FirstOrDefault(x => x.Key == t.Key && x.Slot == t.Slot.ToString());
            if (row == null) { row = new ChecklistTemplate { BusinessId = Business, Key = t.Key, Slot = t.Slot.ToString() }; db.Add(row); }
            row.Description = t.Description.Trim(); row.Priority = t.Priority; row.IsActive = t.IsActive;
        }
        await db.SaveChangesAsync(); return templates;
    }
    public async Task<ChecklistSummaryDto> GetChecklistSummaryAsync(DateTime? startDate, DateTime? endDate)
    {
        var start = DateOnly.FromDateTime(startDate ?? DateTime.UtcNow); var end = DateOnly.FromDateTime(endDate ?? DateTime.UtcNow);
        if (end < start || end.DayNumber - start.DayNumber > 366) throw new ArgumentException("Invalid date range.");
        var templates = (await GetChecklistTemplatesAsync()).Where(x => x.IsActive).ToList();
        var done = await db.Set<ChecklistCompletion>().Where(x => x.BusinessId == Business && x.Date >= start && x.Date <= end).ToListAsync();
        var valid = done.Where(c => templates.Any(t => t.Key == c.TaskKey && t.Slot.ToString() == c.Slot)).DistinctBy(x => (x.Date, x.Slot, x.TaskKey)).ToList();
        var days = end.DayNumber - start.DayNumber + 1;
        return new() { StartDate = start.ToDateTime(TimeOnly.MinValue), EndDate = end.ToDateTime(TimeOnly.MinValue), TotalDays = days,
            TotalTasks = templates.Count * days, CompletedTasks = valid.Count,
            CompletedDays = valid.GroupBy(x => x.Date).Count(g => g.Count() == templates.Count),
            AverageCompletionRate = templates.Count == 0 ? 0 : decimal.Round(100m * valid.Count / (days * templates.Count), 1),
            FrequentlySkippedTasks = templates.Where(t => valid.Count(c => c.TaskKey == t.Key && c.Slot == t.Slot.ToString()) < days).Select(t => t.Description).ToList() };
    }
    public async Task<UsageTodayDto> GetTodayUsageAsync()
    {
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive).OrderBy(x => x.Name).ToListAsync();
        var logs = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == Today).ToDictionaryAsync(x => x.CatalogItemId);
        var purchased = await PurchasedOnAsync(Today);
        return new() { Date = Today.ToDateTime(TimeOnly.MinValue), TotalItems = logs.Count, TotalQuantityUsed = logs.Values.Sum(x => x.UsedQty),
            Lines = items.Select(i => {
                logs.TryGetValue(i.Id, out var l);
                return new UsageLineDto { CatalogItemId = i.Id, ItemName = i.Name, ItemCode = i.ItemCode, Unit = i.DefaultUnit, ExpectedVersion = i.RowVersion,
                    OpeningQty = l?.OpeningQty ?? Math.Max(0, i.CurrentStock - purchased.GetValueOrDefault(i.Id)), PurchasedQty = purchased.GetValueOrDefault(i.Id), QuantityUsed = l?.UsedQty ?? 0, ClosingQty = i.CurrentStock,
                    Notes = l?.Notes, LoggedAt = l?.LoggedAt ?? default };
            }).ToList() };
    }
    public async Task<UsageSummaryDto> SubmitUsageAsync(UsageSubmitDto dto)
    {
        if (!user.HasPermission("stock.adjust") && user.Role is not ("Owner" or "SuperAdmin")) throw new UnauthorizedAccessException();
        if (dto.Lines == null || dto.Lines.Count is < 1 or > 200 || dto.Lines.Any(x => x == null) || dto.Lines.Select(x => x.CatalogItemId).Distinct().Count() != dto.Lines.Count
            || dto.Lines.Any(x => x.CatalogItemId == Guid.Empty || x.ExpectedVersion == Guid.Empty || x.QuantityUsed < 0 || x.QuantityUsed > 1000000000m || decimal.Round(x.QuantityUsed, 4) != x.QuantityUsed || x.Notes?.Length > 1000)) throw new ArgumentException("Invalid usage batch.");
        await using var transaction = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync() : null;
        var ids = dto.Lines.Select(x => x.CatalogItemId).ToArray();
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive && ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var logs = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == Today && ids.Contains(x.CatalogItemId)).ToDictionaryAsync(x => x.CatalogItemId);
        var purchased = await PurchasedOnAsync(Today);
        // Validate the entire batch before any stock write. Submitted values are cumulative daily usage.
        foreach (var line in dto.Lines)
        {
            if (!items.TryGetValue(line.CatalogItemId, out var item)) throw new KeyNotFoundException("Item not found.");
            logs.TryGetValue(item.Id, out var log);
            if (item.RowVersion != line.ExpectedVersion) throw new DbUpdateConcurrencyException("Stock changed; reload before saving usage.");
            if (line.QuantityUsed - (log?.UsedQty ?? 0) > item.CurrentStock - item.ReservedStock) throw new ArgumentException("Usage exceeds available stock.");
            if (item.CurrentStock + (log?.UsedQty ?? 0) - line.QuantityUsed > 1000000000m) throw new ArgumentException("Stock exceeds the supported range.");
        }
        foreach (var line in dto.Lines)
        {
            var item = items[line.CatalogItemId]; logs.TryGetValue(item.Id, out var log);
            var delta = (log?.UsedQty ?? 0) - line.QuantityUsed;
            if (log == null) { log = new DailyUsageLog { BusinessId = Business, CatalogItemId = item.Id, Date = Today, OpeningQty = Math.Max(0, item.CurrentStock - purchased.GetValueOrDefault(item.Id)), LoggedByUserId = User }; db.Add(log); }
            log.PurchasedQty = purchased.GetValueOrDefault(item.Id);
            log.UsedQty = line.QuantityUsed; log.ClosingQty = item.CurrentStock + delta; log.Notes = line.Notes; log.LoggedAt = DateTime.UtcNow; log.LoggedByUserId = User;
            if (delta != 0) await stock.AdjustStockAsync(item.Id, new AdjustStockRequestDto { QuantityDelta = delta, ExpectedVersion = line.ExpectedVersion, Reason = "Daily usage log", Notes = line.Notes, ReferenceType = "DailyUsage", ReferenceId = log.Id.ToString() });
        }
        await db.SaveChangesAsync(); if (transaction != null) await transaction.CommitAsync();
        return await GetUsageSummaryAsync(null);
    }
    public async Task<UsageSummaryDto> GetUsageSummaryAsync(DateTime? date)
    {
        var day = DateOnly.FromDateTime(date ?? DateTime.UtcNow);
        var logs = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == day).ToListAsync();
        var active = await db.CatalogItems.CountAsync(x => x.BusinessId == Business && x.IsActive && x.CurrentStock > 0);
        return new() { Date = day.ToDateTime(TimeOnly.MinValue), TotalItems = logs.Count, MissingItems = Math.Max(0, active - logs.Count), TotalQuantityUsed = logs.Sum(x => x.UsedQty) };
    }
    public async Task<int> MaterializeSnapshotsAsync(DateTime? forDate = null)
    {
        if (!user.HasPermission("stock.adjust") && user.Role is not ("Owner" or "SuperAdmin")) throw new UnauthorizedAccessException();
        var day = DateOnly.FromDateTime(forDate ?? DateTime.UtcNow);
        if (day != Today) throw new ArgumentException("Only today's snapshot can be materialized from current stock.");
        await using var tx = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync() : null;
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive && x.CurrentStock > 0).ToListAsync();
        var existingIds = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == day).Select(x => x.CatalogItemId).ToListAsync();
        var prior = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == day.AddDays(-1)).ToDictionaryAsync(x => x.CatalogItemId);
        var purchased = await PurchasedOnAsync(day);
        var created = 0;
        foreach (var item in items.Where(x => !existingIds.Contains(x.Id)))
        {
            var bought = purchased.GetValueOrDefault(item.Id);
            var opening = prior.TryGetValue(item.Id, out var last) ? last.ClosingQty : Math.Max(0, item.CurrentStock - bought);
            db.Add(new DailyUsageLog { BusinessId = Business, CatalogItemId = item.Id, Date = day, OpeningQty = opening,
                PurchasedQty = bought, ClosingQty = opening + bought, LoggedByUserId = User, LoggedAt = DateTime.UtcNow });
            created++;
        }
        await db.SaveChangesAsync();
        var usage = await GetUsageSummaryAsync(null); var checklist = await GetChecklistSummaryAsync(null, null); var report = await GetOperationsReportSummaryAsync();
        var row = await db.DailyOperationSnapshots.SingleOrDefaultAsync(x => x.BusinessId == Business && x.Date == day);
        if (row == null) { row = new DailyOperationSnapshot { BusinessId = Business, Date = day }; db.Add(row); }
        row.TotalItemsUsed = usage.TotalItems; row.TotalQuantityUsed = usage.TotalQuantityUsed;
        row.TotalChecklistTasks = checklist.TotalTasks; row.ChecklistCompletionRate = checklist.AverageCompletionRate;
        row.CompletedChecklistTasks = checklist.CompletedTasks;
        row.DeadStockItems = report.Items.Count(x => x.MovementStatus == "dead"); row.FastMovingItems = report.Items.Count(x => x.MovementStatus == "fast");
        row.SlowMovingItems = report.Items.Count(x => x.MovementStatus is "slow" or "very_slow"); row.MaterializedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        if (tx != null) await tx.CommitAsync();
        return created;
    }
    private async Task<Dictionary<Guid, decimal>> PurchasedOnAsync(DateOnly day)
    {
        var start = DateTime.SpecifyKind(day.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc); var end = start.AddDays(1);
        return await db.StockMovements.Where(x => x.BusinessId == Business && x.ReferenceType == "PurchaseOrder" && x.QuantityDelta > 0 && x.CreatedAt >= start && x.CreatedAt < end)
            .GroupBy(x => x.CatalogItemId).Select(g => new { Id = g.Key, Qty = g.Sum(x => x.QuantityDelta) }).ToDictionaryAsync(x => x.Id, x => x.Qty);
    }
    public async Task<List<DailyUsageSnapshotDto>> GetSnapshotsAsync(DateTime? fromDate, DateTime? toDate, Guid? itemId)
    {
        var from = DateOnly.FromDateTime(fromDate ?? DateTime.UtcNow); var to = DateOnly.FromDateTime(toDate ?? DateTime.UtcNow);
        if (to < from || to.DayNumber - from.DayNumber > 366) throw new ArgumentException("Invalid date range.");
        if (itemId.HasValue && !await db.CatalogItems.AnyAsync(x => x.BusinessId == Business && x.Id == itemId)) throw new KeyNotFoundException();
        return await db.DailyUsageLogs.AsNoTracking().Where(x => x.BusinessId == Business && x.Date >= from && x.Date <= to && (!itemId.HasValue || x.CatalogItemId == itemId))
            .OrderByDescending(x => x.Date).ThenBy(x => x.CatalogItem.Name).Take(500).Select(x => new DailyUsageSnapshotDto {
                Date = x.Date.ToDateTime(TimeOnly.MinValue), CatalogItemId = x.CatalogItemId, ItemName = x.CatalogItem.Name, Unit = x.CatalogItem.DefaultUnit,
                OpeningQty = x.OpeningQty, PurchasedQty = x.PurchasedQty, QuantityUsed = x.UsedQty, ClosingQty = x.ClosingQty, Notes = x.Notes, LoggedAt = x.LoggedAt
            }).ToListAsync();
    }
    private bool SeesAllTasks => user.Role is "Owner" or "Admin" or "Manager" or "SuperAdmin";
    private void RequireTaskOwner() { if (user.Role is not ("Owner" or "Admin" or "SuperAdmin")) throw new UnauthorizedAccessException(); }
    public async Task<List<TaskAssigneeDto>> GetTaskAssigneesAsync()
    {
        RequireTaskOwner();
        return await db.Memberships.Where(x => x.BusinessId == Business && x.User.Status == UserStatus.Active)
            .OrderBy(x => x.User.Name).Select(x => new TaskAssigneeDto(x.UserId, x.User.Name)).ToListAsync();
    }
    public async Task<List<StaffTaskDto>> GetStaffTasksAsync(string? status, Guid? staffId)
    {
        if (status != null && status is not ("assigned" or "accepted" or "completed" or "rejected")) throw new ArgumentException("Invalid task status.");
        if (!SeesAllTasks && staffId.HasValue && staffId != User) throw new UnauthorizedAccessException();
        var tasks = await db.Set<StaffTask>().AsNoTracking().Where(x => x.BusinessId == Business && (SeesAllTasks || x.StaffId == User)
            && (!staffId.HasValue || x.StaffId == staffId) && (status == null || x.Status == status)).OrderByDescending(x => x.AssignedAt).Take(500).ToListAsync();
        var names = await db.Memberships.Where(x => x.BusinessId == Business).Select(x => new { x.UserId, x.User.Name }).ToDictionaryAsync(x => x.UserId, x => x.Name);
        return tasks.Select(x => MapTask(x, names.GetValueOrDefault(x.StaffId, "Business member"))).ToList();
    }
    private static StaffTaskDto MapTask(StaffTask x, string name) => new() { Id = x.Id, StaffId = x.StaffId, StaffName = name, TaskType = x.TaskType,
        ReferenceId = x.ReferenceId, Status = x.Status, Rejected = x.Rejected, CorrectionNote = x.CorrectionNote, AssignedAt = x.AssignedAt,
        AcceptedAt = x.AcceptedAt, CompletedAt = x.CompletedAt, Version = x.Version };
    public async Task<StaffTaskDto> CreateStaffTaskAsync(StaffTaskCreateDto dto)
    {
        RequireTaskOwner();
        if (dto.StaffId == Guid.Empty || string.IsNullOrWhiteSpace(dto.TaskType) || dto.TaskType.Length > 64 || dto.ReferenceId?.Length > 255) throw new ArgumentException("Invalid task.");
        var assignee = await db.Memberships.Where(x => x.BusinessId == Business && x.UserId == dto.StaffId && x.User.Status == UserStatus.Active).Select(x => x.User.Name).SingleOrDefaultAsync();
        if (assignee == null) throw new KeyNotFoundException("Active business member not found.");
        var task = new StaffTask { BusinessId = Business, StaffId = dto.StaffId, TaskType = dto.TaskType.Trim(), ReferenceId = dto.ReferenceId?.Trim(), CreatedById = User };
        db.Add(task); await db.SaveChangesAsync(); return MapTask(task, assignee);
    }
    public async Task<StaffTaskDto> ActOnStaffTaskAsync(Guid id, StaffTaskActionDto dto, bool accept)
    {
        if (dto.ExpectedVersion == Guid.Empty || dto.CorrectionNote?.Length > 1000) throw new ArgumentException("Invalid task action.");
        var task = await db.Set<StaffTask>().SingleOrDefaultAsync(x => x.Id == id && x.BusinessId == Business) ?? throw new KeyNotFoundException();
        if (task.StaffId != User) throw new UnauthorizedAccessException();
        if (task.Version != dto.ExpectedVersion || task.Status is "completed" or "rejected" || (accept && task.Status != "assigned")) throw new DbUpdateConcurrencyException("Task has already changed.");
        if (accept) { task.Status = "accepted"; task.AcceptedAt = DateTime.UtcNow; }
        else { task.Status = dto.Rejected ? "rejected" : "completed"; task.Rejected = dto.Rejected; task.CorrectionNote = dto.CorrectionNote?.Trim(); task.CompletedAt = DateTime.UtcNow; }
        task.Version = Guid.NewGuid(); task.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync();
        var name = await db.Users.Where(x => x.Id == task.StaffId).Select(x => x.Name).SingleAsync(); return MapTask(task, name);
    }
    public async Task<List<StaffPerformanceDto>> GetStaffPerformanceAsync()
    {
        RequireTaskOwner(); var tasks = await GetStaffTasksAsync(null, null);
        return tasks.GroupBy(x => (x.StaffId, x.StaffName)).Select(g => new StaffPerformanceDto(g.Key.StaffId, g.Key.StaffName, g.Count(),
            g.Count(x => x.Status == "completed"), g.Count(x => x.Status is "assigned" or "accepted"), g.Count(x => x.Status == "rejected"))).ToList();
    }
    public async Task<OwnerOperationsDashboardDto> GetOwnerDashboardAsync()
    {
        RequireTaskOwner();
        var start = DateTime.SpecifyKind(Today.AddDays(-6).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc); var end = start.AddDays(7);
        var result = new OwnerOperationsDashboardDto { AsOf = DateTime.UtcNow,
            LowStockCount = await db.CatalogItems.CountAsync(x => x.BusinessId == Business && x.IsActive && x.CurrentStock <= x.ReorderLevel),
            OutOfStockCount = await db.CatalogItems.CountAsync(x => x.BusinessId == Business && x.IsActive && x.CurrentStock <= 0),
            PendingDamageCount = await db.PurchaseDamageReports.CountAsync(x => x.BusinessId == Business && x.Status == "pending"),
            StaffPerformance = await GetStaffPerformanceAsync() };
        var todayUtc = DateTime.UtcNow.Date;
        result.AiRequestsToday = await db.AiUsageLogs.CountAsync(x => x.BusinessId == Business && x.CreatedAt >= todayUtc && x.CreatedAt < todayUtc.AddDays(1));
        var latestBackup = await db.BackupLogs.AsNoTracking().Where(x => x.BusinessId == Business).OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).FirstOrDefaultAsync();
        result.BackupLastStatus = latestBackup?.Status; result.BackupLastAt = latestBackup?.CreatedAt; result.BackupLastSizeBytes = latestBackup?.SizeBytes;
        if (latestBackup == null) result.Exceptions.Add("No server backup has been recorded");
        else if (latestBackup.Status != "success") result.Exceptions.Add("Latest server backup failed");
        if (user.Role is "Owner" or "SuperAdmin") result.SpendLast7Days = await db.Purchases.Where(x => x.BusinessId == Business && x.CreatedAt >= start && x.CreatedAt < end && x.Status != PurchaseStatus.Cancelled && x.Status != PurchaseStatus.Draft).SumAsync(x => x.GrandTotal);
        if (result.OutOfStockCount > 0) result.Exceptions.Add($"{result.OutOfStockCount} items at zero stock");
        if (result.LowStockCount > 0) result.Exceptions.Add($"{result.LowStockCount} items at or below reorder level");
        if (result.PendingDamageCount > 0) result.Exceptions.Add($"{result.PendingDamageCount} damage reports awaiting review");
        var pending = result.StaffPerformance.Sum(x => x.Pending); if (pending > 0) result.Exceptions.Add($"{pending} staff tasks still open");
        return result;
    }
    public async Task<OperationsReportSummaryDto> GetOperationsReportSummaryAsync()
    {
        var items = await db.CatalogItems.Include(x => x.Category).Where(x => x.BusinessId == Business && x.IsActive).ToListAsync();
        var since = Today.AddDays(-30);
        var usage = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date >= since).ToListAsync();
        var movements = await db.StockMovements.Where(x => x.BusinessId == Business).GroupBy(x => x.CatalogItemId)
            .Select(g => new { Id = g.Key, Last = g.Max(x => x.CreatedAt), Purchase = g.Where(x => x.ReferenceType == "PurchaseOrder").Max(x => (DateTime?)x.CreatedAt) }).ToDictionaryAsync(x => x.Id);
        var result = new OperationsReportSummaryDto();
        foreach (var i in items)
        {
            var used7 = usage.Where(x => x.CatalogItemId == i.Id && x.Date >= Today.AddDays(-7)).Sum(x => x.UsedQty);
            var used30 = usage.Where(x => x.CatalogItemId == i.Id).Sum(x => x.UsedQty);
            movements.TryGetValue(i.Id, out var movement);
            var idle = used7 > 0 ? 0 : movement == null ? 999 : Math.Max(0, (DateTime.UtcNow - movement.Last).Days);
            var dead = i.CurrentStock > 0 && used7 <= 0 && (movement?.Purchase == null || (DateTime.UtcNow - movement.Purchase.Value).Days >= 30);
            var status = i.CurrentStock <= 0 ? "out_of_stock" : dead || idle >= 60 ? "dead" : used7 > 0 ? "fast" : idle >= 30 ? "very_slow" : idle >= 7 ? "slow" : "active";
            var bucket = i.CurrentStock <= 0 ? "out" : used7 > 0 ? "healthy" : idle >= 60 ? "60d" : idle >= 30 ? "30d" : idle >= 15 ? "15d" : idle >= 7 ? "7d" : "healthy";
            result.Items.Add(new() { Id = i.Id, Name = i.Name, ItemCode = i.ItemCode, Category = i.Category?.Name ?? "", Unit = i.DefaultUnit,
                CurrentStock = i.CurrentStock, Used7d = used7, Used30d = used30, IdleDays = idle, MovementStatus = status, LastMovementAt = movement?.Last,
                AgingBucket = bucket, InsightKey = bucket switch { "out" => "out_of_stock", "60d" => "dead_stock_risk", "30d" => "high_stock_low_usage", "15d" or "7d" => "slowing", _ => "active" } });
        }
        result.Items = result.Items.OrderByDescending(x => x.CurrentStock).ToList();
        result.Summary = new() { ["all"] = result.Items.Count(x => x.CurrentStock > 0) };
        foreach (var key in new[] { "active", "slow", "dead", "fast", "no_activity" }) result.Summary[key] = result.Items.Count(x => key == "slow" ? x.MovementStatus is "slow" or "very_slow" : x.MovementStatus == key);
        result.DeadStock = result.Items.Where(x => x.MovementStatus == "dead").Take(50).ToList();
        result.FastMoving = result.Items.Where(x => x.Used7d > 0).OrderByDescending(x => x.Used7d).Take(30).ToList();
        result.SlowMoving = result.Items.Where(x => x.CurrentStock > 0 && x.Used7d <= 0).Take(30).ToList();
        var month = DateTime.SpecifyKind(new DateTime(Today.Year, Today.Month, 1), DateTimeKind.Utc);
        result.SupplierFrequency = await db.Purchases.Where(x => x.BusinessId == Business && x.Status != PurchaseStatus.Cancelled && x.CreatedAt >= month)
            .GroupBy(x => new { x.SupplierId, x.Supplier.Name }).OrderByDescending(g => g.Count()).Take(20).Select(g => new SupplierFrequencyDto(g.Key.SupplierId, g.Key.Name, g.Count())).ToListAsync();
        return result;
    }
}
