using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Operations;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using System.Text.RegularExpressions;

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
            return new ChecklistTaskDto { Key = t.Key, Description = t.Description, Priority = t.Priority, IsCompleted = c != null, CompletedAt = c?.CompletedAt };
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
        if (templates.Count is < 1 or > 100 || templates.Any(x => !Enum.IsDefined(x.Slot) || string.IsNullOrWhiteSpace(x.Description) || x.Description.Length > 255 || !Regex.IsMatch(x.Key, "^[a-z0-9_]{1,64}$"))
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
            CompletedDays = valid.GroupBy(x => x.Date).Count(g => g.Count() == templates.Count),
            AverageCompletionRate = templates.Count == 0 ? 0 : decimal.Round(100m * valid.Count / (days * templates.Count), 1),
            FrequentlySkippedTasks = templates.Where(t => valid.Count(c => c.TaskKey == t.Key && c.Slot == t.Slot.ToString()) < days).Select(t => t.Description).ToList() };
    }
    public async Task<UsageTodayDto> GetTodayUsageAsync()
    {
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive).OrderBy(x => x.Name).ToListAsync();
        var logs = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == Today).ToDictionaryAsync(x => x.CatalogItemId);
        return new() { Date = Today.ToDateTime(TimeOnly.MinValue), TotalItems = logs.Count, TotalQuantityUsed = logs.Values.Sum(x => x.UsedQty),
            Lines = items.Select(i => {
                logs.TryGetValue(i.Id, out var l);
                return new UsageLineDto { CatalogItemId = i.Id, ItemName = i.Name, ItemCode = i.ItemCode, Unit = i.DefaultUnit, ExpectedVersion = i.RowVersion,
                    OpeningQty = l?.OpeningQty ?? i.CurrentStock, PurchasedQty = l?.PurchasedQty ?? 0, QuantityUsed = l?.UsedQty ?? 0, ClosingQty = i.CurrentStock,
                    Notes = l?.Notes, LoggedAt = l?.LoggedAt ?? default };
            }).ToList() };
    }
    public async Task<UsageSummaryDto> SubmitUsageAsync(UsageSubmitDto dto)
    {
        if (!user.HasPermission("stock.adjust") && user.Role is not ("Owner" or "SuperAdmin")) throw new UnauthorizedAccessException();
        if (dto.Lines.Count is < 1 or > 200 || dto.Lines.Select(x => x.CatalogItemId).Distinct().Count() != dto.Lines.Count
            || dto.Lines.Any(x => x.QuantityUsed < 0 || x.QuantityUsed > 1000000000m || decimal.Round(x.QuantityUsed, 4) != x.QuantityUsed || x.Notes?.Length > 1000)) throw new ArgumentException("Invalid usage batch.");
        await using var transaction = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync() : null;
        var ids = dto.Lines.Select(x => x.CatalogItemId).ToArray();
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive && ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        var logs = await db.DailyUsageLogs.Where(x => x.BusinessId == Business && x.Date == Today && ids.Contains(x.CatalogItemId)).ToDictionaryAsync(x => x.CatalogItemId);
        // Validate the entire batch before any stock write. Submitted values are cumulative daily usage.
        foreach (var line in dto.Lines)
        {
            if (!items.TryGetValue(line.CatalogItemId, out var item)) throw new KeyNotFoundException("Item not found.");
            logs.TryGetValue(item.Id, out var log);
            if (item.RowVersion != line.ExpectedVersion) throw new DbUpdateConcurrencyException("Stock changed; reload before saving usage.");
            if (line.QuantityUsed - (log?.UsedQty ?? 0) > item.CurrentStock - item.ReservedStock) throw new ArgumentException("Usage exceeds available stock.");
        }
        foreach (var line in dto.Lines)
        {
            var item = items[line.CatalogItemId]; logs.TryGetValue(item.Id, out var log);
            var delta = (log?.UsedQty ?? 0) - line.QuantityUsed;
            if (log == null) { log = new DailyUsageLog { BusinessId = Business, CatalogItemId = item.Id, Date = Today, OpeningQty = item.CurrentStock, LoggedByUserId = User }; db.Add(log); }
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
        return new() { Date = day.ToDateTime(TimeOnly.MinValue), TotalItems = logs.Count, TotalQuantityUsed = logs.Sum(x => x.UsedQty) };
    }
    public async Task<int> MaterializeSnapshotsAsync(DateTime? forDate = null)
    {
        var day = DateOnly.FromDateTime(forDate ?? DateTime.UtcNow);
        if (day != Today) throw new ArgumentException("Only today's snapshot can be materialized from current stock.");
        var usage = await GetUsageSummaryAsync(null); var checklist = await GetChecklistSummaryAsync(null, null); var report = await GetOperationsReportSummaryAsync();
        var row = await db.DailyOperationSnapshots.SingleOrDefaultAsync(x => x.BusinessId == Business && x.Date == day);
        if (row == null) { row = new DailyOperationSnapshot { BusinessId = Business, Date = day }; db.Add(row); }
        row.TotalItemsUsed = usage.TotalItems; row.TotalQuantityUsed = usage.TotalQuantityUsed;
        row.TotalChecklistTasks = (await GetChecklistTemplatesAsync()).Count(x => x.IsActive); row.ChecklistCompletionRate = checklist.AverageCompletionRate;
        row.CompletedChecklistTasks = (int)(row.TotalChecklistTasks * row.ChecklistCompletionRate / 100m);
        row.DeadStockItems = report.Items.Count(x => x.MovementStatus == "dead"); row.FastMovingItems = report.Items.Count(x => x.MovementStatus == "fast");
        row.SlowMovingItems = report.Items.Count(x => x.MovementStatus is "slow" or "very_slow"); row.MaterializedAt = DateTime.UtcNow;
        return await db.SaveChangesAsync();
    }
    public async Task<OperationsReportSummaryDto> GetOperationsReportSummaryAsync()
    {
        var items = await db.CatalogItems.Where(x => x.BusinessId == Business && x.IsActive).ToListAsync();
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
            result.Items.Add(new() { Id = i.Id, Name = i.Name, CurrentStock = i.CurrentStock, Used7d = used7, Used30d = used30, IdleDays = idle, MovementStatus = status });
        }
        return result;
    }
}
