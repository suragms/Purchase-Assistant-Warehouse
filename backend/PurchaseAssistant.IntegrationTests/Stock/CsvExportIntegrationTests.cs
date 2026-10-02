using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.IntegrationTests.Stock;
public partial class StockServiceIntegrationTests
{
    private async Task<(CatalogItem Item, Supplier Supplier)> SeedCsvPg(string unit = "BAG", decimal? kg = 50.1234m)
    {
        var item = await CreateItemAsync(current: 12.3456m);
        // The fixture's non-stock changes use tracked entities; stock itself is never assigned.
        _context.Attach(item); item.Name = "Item \"Premium\", മലയാളം"; item.DefaultUnit = unit; item.ReorderLevel = 20m;
        var supplier = new Supplier { BusinessId = _businessId, Name = "ABC, Store മലയാളം" };
        _context.Suppliers.Add(supplier);
        _context.Purchases.Add(new PurchaseOrder { BusinessId = _businessId, Supplier = supplier, OrderNumber = "CSV-1", Status = PurchaseStatus.Completed,
            GrandTotal = 9999m, CreatedAt = DateTime.UtcNow, Items = [new PurchaseItem { BusinessId = _businessId, CatalogItemId = item.Id,
                Unit = unit, KgPerUnit = kg, OrderedQuantity = 2.3456m, ReceivedQuantity = 0.4567m, UnitPrice = 100.1234m, LandingCostPerKg = 1.2345m, LineTotal = 10.5000m }] });
        await _context.SaveChangesAsync(); return (item, supplier);
    }
    [Fact]
    public async Task CsvPostgresQueriesUseCanonicalReceiptsTotalsAndDoNotWriteStock()
    {
        var data = await SeedCsvPg(); var reports = new ReportService(_context); var stock = new StockService(_context, _user);
        var rows = await stock.GetCsvRowsAsync("low-stock", null, null, null, [data.Item.Id], default);
        var row = Assert.Single(rows); Assert.Equal(12.3456m, row.Current); Assert.Equal(0.4567m, row.Purchased); Assert.Null(row.LastMovement);
        var line = Assert.Single(await reports.GetCsvPurchaseLinesAsync(_businessId, null, null, data.Supplier.Id, default)); Assert.Equal(10.5000m, line.LineTotal);
        var report = await reports.GetCsvPackReportsAsync(_businessId, null, null, default);
        Assert.Equal(10.5000m, Assert.Single(report.Suppliers).Amount); var item = Assert.Single(report.Items);
        Assert.Equal(2.3456m, item.Bags); Assert.Equal(2.3456m * 50.1234m, item.Kg); Assert.Equal(1, item.Purchases);
        Assert.Empty(await reports.GetCsvPurchaseLinesAsync(_businessId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), null, default));
        Assert.Equal(12.3456m, (await _context.CatalogItems.AsNoTracking().SingleAsync(x => x.Id == data.Item.Id)).CurrentStock); Assert.Equal(0, await _context.StockMovements.CountAsync());
    }
    [Fact]
    public async Task CsvPostgresNeverIncludesOtherBusinessLinesSuppliersStockOrAggregates()
    {
        var own = await SeedCsvPg(); var foreign = Guid.NewGuid(); Guid otherItem = Guid.Empty;
        try
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, new StubTenant(foreign));
            var business = new Business { Id = foreign, Name = "CSV FOREIGN PRIVATE" }; var category = new Category { BusinessId = foreign, Name = "CSV FOREIGN PRIVATE" };
            var supplier = new Supplier { BusinessId = foreign, Name = "CSV FOREIGN PRIVATE" }; var item = new CatalogItem { BusinessId = foreign, Category = category,
                Name = "CSV FOREIGN PRIVATE", ItemCode = "FOREIGN", DefaultUnit = "BAG", CurrentStock = 99m, ReorderLevel = 100m };
            other.Businesses.Add(business); other.CatalogItems.Add(item); other.Purchases.Add(new PurchaseOrder { BusinessId = foreign, Supplier = supplier, OrderNumber = "FOREIGN", Status = PurchaseStatus.Completed,
                Items = [new PurchaseItem { BusinessId = foreign, CatalogItem = item, Unit = "BAG", KgPerUnit = 100m, OrderedQuantity = 999m, ReceivedQuantity = 999m, LineTotal = 999999m }] });
            await other.SaveChangesAsync(); otherItem = item.Id;
            var stock = new StockService(_context, _user); var reports = new ReportService(_context);
            foreach (var filter in new[] { "all", "low-stock" }) { var rows = await stock.GetCsvRowsAsync(filter, null, null, null, null, default); Assert.Single(rows); Assert.DoesNotContain(rows, x => x.Id == otherItem); }
            await Assert.ThrowsAsync<KeyNotFoundException>(() => stock.GetCsvRowsAsync("all", null, null, null, [otherItem], default));
            Assert.Empty(await reports.GetCsvPurchaseLinesAsync(_businessId, null, null, supplier.Id, default));
            var line = Assert.Single(await reports.GetCsvPurchaseLinesAsync(_businessId, null, null, null, default)); Assert.Equal(own.Supplier.Id, line.SupplierId);
            var report = await reports.GetCsvPackReportsAsync(_businessId, null, null, default); Assert.Equal(10.5m, Assert.Single(report.Suppliers).Amount); Assert.Single(report.Items);
        }
        finally
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"PurchaseItems\" WHERE \"BusinessId\" = {0}", foreign);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Purchases\" WHERE \"BusinessId\" = {0}", foreign);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogItems\" WHERE \"BusinessId\" = {0}", foreign);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Suppliers\" WHERE \"BusinessId\" = {0}", foreign);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Categories\" WHERE \"BusinessId\" = {0}", foreign);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Businesses\" WHERE \"Id\" = {0}", foreign);
        }
    }
    [Fact]
    public async Task CsvPostgresMissingWeightAndUnknownConversionStayNullAndLedgerSuppliesTimestamp()
    {
        var data = await SeedCsvPg("BAG", null); var reports = new ReportService(_context); var stock = new StockService(_context, _user);
        var report = await reports.GetCsvPackReportsAsync(_businessId, null, null, default); Assert.Null(Assert.Single(report.Items).Kg); Assert.Null(Assert.Single(report.Suppliers).BagKg);
        await stock.UpdatePhysicalStockAsync(data.Item.Id, new() { PhysicalStock = 2.4567m, ExpectedVersion = data.Item.RowVersion, Reason = "CSV verification count" });
        var line = await _context.PurchaseItems.SingleAsync(); line.Unit = "KG"; await _context.SaveChangesAsync();
        var row = Assert.Single(await stock.GetCsvRowsAsync("all", null, null, null, null, default)); Assert.Null(row.Purchased); Assert.NotNull(row.LastMovement); Assert.Equal(2.4567m, row.Physical); Assert.Equal(12.3456m, row.Current);
    }
}
