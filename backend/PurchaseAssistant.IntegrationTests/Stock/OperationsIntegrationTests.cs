using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Operations;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.IntegrationTests.Stock;

public partial class StockServiceIntegrationTests
{
    [RequiresDisposablePostgresFact]
    public async Task DailyUsageUsesTheStockLedgerAndRejectsAStaleRepeat()
    {
        var item = await CreateItemAsync(current: 20);
        var operations = new OperationsService(_context, _user, new StockService(_context, _user));
        await operations.SubmitUsageAsync(new() { Lines = [new() { CatalogItemId = item.Id, ExpectedVersion = item.RowVersion, QuantityUsed = 3 }] });
        Assert.Equal(17, (await _context.CatalogItems.SingleAsync(x => x.Id == item.Id)).CurrentStock);
        var movement = await _context.StockMovements.SingleAsync(x => x.CatalogItemId == item.Id);
        Assert.Equal("DailyUsage", movement.ReferenceType); Assert.Equal(-3, movement.QuantityDelta);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => operations.SubmitUsageAsync(new() { Lines = [new() { CatalogItemId = item.Id, ExpectedVersion = item.RowVersion, QuantityUsed = 4 }] }));
        Assert.Equal(1, await _context.StockMovements.CountAsync(x => x.CatalogItemId == item.Id));
    }
    [RequiresDisposablePostgresFact]
    public async Task DailyUsageValidatesTheWholeBatchBeforeWritingAndSnapshotsAreIdempotent()
    {
        var item = await CreateItemAsync(current: 20);
        var operations = new OperationsService(_context, _user, new StockService(_context, _user));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => operations.SubmitUsageAsync(new() { Lines = [
            new() { CatalogItemId = item.Id, ExpectedVersion = item.RowVersion, QuantityUsed = 2 },
            new() { CatalogItemId = Guid.NewGuid(), ExpectedVersion = Guid.NewGuid(), QuantityUsed = 1 }] }));
        Assert.Equal(20, (await _context.CatalogItems.SingleAsync(x => x.Id == item.Id)).CurrentStock);
        Assert.Empty(await _context.StockMovements.Where(x => x.CatalogItemId == item.Id).ToListAsync());
        await operations.CompleteChecklistTaskAsync(ChecklistSlot.Morning, "open_check", new());
        await operations.CompleteChecklistTaskAsync(ChecklistSlot.Morning, "fridge_temp", new());
        Assert.Equal(1, await operations.MaterializeSnapshotsAsync());
        Assert.Equal(0, await operations.MaterializeSnapshotsAsync());
        var snapshot = await _context.DailyOperationSnapshots.SingleAsync();
        Assert.Equal(6, snapshot.TotalChecklistTasks);
        Assert.Equal(2, snapshot.CompletedChecklistTasks); // A rounded 33.3% must not truncate this to one.
        Assert.Single(await operations.GetSnapshotsAsync(null, null, item.Id));
        Assert.Equal(20, (await _context.CatalogItems.SingleAsync(x => x.Id == item.Id)).CurrentStock);
        Assert.Empty(await _context.StockMovements.Where(x => x.CatalogItemId == item.Id).ToListAsync());
    }
}
