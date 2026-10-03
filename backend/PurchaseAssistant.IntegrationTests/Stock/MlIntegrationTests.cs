using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.ML;

namespace PurchaseAssistant.IntegrationTests.Stock;

public partial class StockServiceIntegrationTests
{
    [RequiresDisposablePostgresFact]
    public async Task ForecastTrainingInferenceAndMonitoringUsePersistedConfirmedHistory()
    {
        var item = await CreateItemAsync(current: 10); var now = DateTime.UtcNow; var today = DateOnly.FromDateTime(now);
        var root = Path.Combine(Path.GetTempPath(), "wa-ml-integration-" + Guid.NewGuid().ToString("N"));
        try {
            var observations = Enumerable.Range(0, 180).Select(i => { var date = today.AddDays(i - 180); return new UsageObservation(date, 20 + .15 * i + 2 * (int)date.DayOfWeek, true, date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc)); }).ToList();
            _context.DailyUsageLogs.AddRange(observations.Select(x => new DailyUsageLog { BusinessId = _businessId, CatalogItemId = item.Id, Date = x.Date, UsedQty = (decimal)x.Quantity!.Value, IsConfirmed = true, LoggedAt = x.RecordedAt, LoggedByUserId = _userId }));
            await _context.SaveChangesAsync();
            var records = await _context.DailyUsageLogs.Select(x => new UsageObservation(x.Date, (double?)x.UsedQty, x.IsConfirmed, x.LoggedAt)).ToListAsync();
            var artifact = ForecastModel.Train(_businessId, item.Id, item.DefaultUnit, UsageData.Prepare(records, today, now), now);
            var store = new ArtifactStore(root); await store.SaveAsync(artifact);
            var service = new MlService(_context, _user, store, TimeProvider.System);
            var prediction = await service.AnalyzeAsync(item.Id, 14, default);
            Assert.Equal("ready", prediction.Status); Assert.Equal(14, prediction.Forecast.Count); Assert.True(prediction.Reorder!.Quantity > 0);
            await service.AnalyzeAsync(item.Id, 14, default); Assert.Equal(1, await _context.MlPredictionLogs.CountAsync());
            var log = await _context.MlPredictionLogs.SingleAsync(); Assert.Equal(artifact.Version, log.ModelVersion); Assert.Equal(artifact.DatasetVersion, log.InputVersion);
            Assert.Equal((decimal)prediction.Forecast.Sum(x => x.Quantity), log.PredictedQuantity, 4);
            Assert.NotNull(await service.MonitoringAsync(item.Id, default));
            // A corrected historical value invalidates its training fingerprint.
            var first = await _context.DailyUsageLogs.OrderBy(x => x.Date).FirstAsync(); first.UsedQty += 1; await _context.SaveChangesAsync();
            Assert.Equal("history_changed", (await service.AnalyzeAsync(item.Id, 7, default)).Status);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AnalyzeAsync(Guid.NewGuid(), 7, default));
        } finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
    [RequiresDisposablePostgresFact]
    public async Task GeneratedSnapshotsAreNotConfirmedDemandAndExplicitZeroIsConfirmed()
    {
        var item = await CreateItemAsync(current: 10); var operations = new OperationsService(_context, _user, new StockService(_context, _user));
        await operations.MaterializeSnapshotsAsync(); Assert.False((await _context.DailyUsageLogs.SingleAsync()).IsConfirmed);
        await operations.SubmitUsageAsync(new() { Lines = [new() { CatalogItemId = item.Id, ExpectedVersion = item.RowVersion, QuantityUsed = 0 }] });
        Assert.True((await _context.DailyUsageLogs.SingleAsync()).IsConfirmed);
    }
    [RequiresDisposablePostgresFact]
    public async Task MutationAuditAndNotificationsCommitAndRollbackWithStock()
    {
        var item = await CreateItemAsync(current: 10);
        (await _context.Users.SingleAsync(x => x.Id == _userId)).Status = UserStatus.Active;
        _context.Memberships.Add(new Membership { BusinessId = _businessId, UserId = _userId, Role = Role.Owner }); await _context.SaveChangesAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
        await using (var db = new AppDbContext(options, _tenant, _user)) {
            var stock = new StockService(db, _user);
            await stock.AdjustStockAsync(item.Id, new() { QuantityDelta = -10, ExpectedVersion = item.RowVersion, Reason = "Integration test" });
            Assert.True(await db.SecurityAuditLogs.AnyAsync(x => x.EventType == "StockMovementAdded"));
            Assert.Single(await db.Notifications.Where(x => x.Type == NotificationType.OutOfStock).ToListAsync());
            var current = await db.CatalogItems.SingleAsync(x => x.Id == item.Id);
            var count = await db.SecurityAuditLogs.CountAsync();
            await using var tx = await db.Database.BeginTransactionAsync();
            await stock.AdjustStockAsync(item.Id, new() { QuantityDelta = 5, ExpectedVersion = current.RowVersion, Reason = "Rollback" });
            await tx.RollbackAsync(); db.ChangeTracker.Clear();
            Assert.Equal(0, (await db.CatalogItems.SingleAsync(x => x.Id == item.Id)).CurrentStock);
            Assert.Equal(count, await db.SecurityAuditLogs.CountAsync());
        }
    }
    [RequiresDisposablePostgresFact]
    public async Task NotificationPreferencesSuppressAutomaticStockAlerts()
    {
        var item = await CreateItemAsync(current: 10);
        (await _context.Users.SingleAsync(x => x.Id == _userId)).Status = UserStatus.Active;
        _context.Memberships.Add(new Membership { BusinessId = _businessId, UserId = _userId, Role = Role.Owner });
        _context.Add(new UserSettings { BusinessId = _businessId, UserId = _userId, NotificationsEnabled = false }); await _context.SaveChangesAsync();
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options, _tenant, _user);
        await new StockService(db, _user).AdjustStockAsync(item.Id, new() { QuantityDelta = -10, ExpectedVersion = item.RowVersion });
        Assert.Empty(await db.Notifications.ToListAsync()); Assert.NotEmpty(await db.SecurityAuditLogs.ToListAsync());
    }
}
