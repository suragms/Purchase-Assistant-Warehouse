using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.FileIO;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.UnitTests.AI;
public partial class PurchaseIntentEndpointTests
{
    private static async Task<(Guid Item, Guid Supplier)> SeedCsv(Factory factory)
    {
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var category = new Category { BusinessId = BusinessId, Name = "Grain" };
        var supplier = new Supplier { BusinessId = BusinessId, Name = "ABC, Store \"മലയാളം\"" };
        var item = new CatalogItem { BusinessId = BusinessId, Category = category, LastSupplier = supplier, Name = "Item \"Premium\", മലയാളം\nNew", ItemCode = "CSV",
            DefaultUnit = "BAG", CurrentStock = 1.2345m, PhysicalStock = 0.5678m, ReorderLevel = 3.1234m };
        var purchase = new PurchaseOrder { BusinessId = BusinessId, Supplier = supplier, OrderNumber = "=DANGEROUS", Status = PurchaseStatus.Confirmed,
            GrandTotal = 999m, Items = [new PurchaseItem { BusinessId = BusinessId, CatalogItem = item, Unit = "BAG", OrderedQuantity = 2.3456m, ReceivedQuantity = 0.4567m,
                KgPerUnit = 50.1234m, LandingCostPerKg = 1.2345m, UnitPrice = 100.1234m, LineTotal = 10.5000m }] };
        db.CatalogItems.Add(item); db.Suppliers.Add(supplier); db.Purchases.Add(purchase); await db.SaveChangesAsync();
        return (item.Id, supplier.Id);
    }
    private static List<string[]> ParseCsv(string text)
    {
        using var parser = new TextFieldParser(new StringReader(text)) { HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = false, CommentTokens = ["#"] };
        parser.SetDelimiters(","); var rows = new List<string[]>(); while (!parser.EndOfData) rows.Add(parser.ReadFields()!); return rows;
    }
    private static string[] CsvPaths(Guid supplier) => ["stock.csv", "low-stock.csv", $"suppliers/{supplier}/purchases.csv", "reports/suppliers.csv", "reports/items.csv"];
    private static string[][] CsvHeaders => [
        ["Item", "Category", "Subcategory", "Unit", "Current Stock", "Opening Stock", "Purchased", "Reorder Level", "Last Updated"],
        ["name", "subcategory", "unit", "system_stock", "physical_stock", "reorder", "purchased", "status", "supplier"],
        ["date", "pur_id", "item", "qty", "unit", "landing_per_unit", "selling", "total_line"],
        ["supplier", "bag_qty", "bag_kg", "amount_inr"], ["name", "kg", "bags", "boxes", "tins", "amount_inr", "purchase_count"] ];
    [Fact]
    public async Task EveryCsvHasExactHeadersUnicodeQuotedRowsAuthoritativeDecimalsAndNoForeignBusinessData()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient();
        var data = await SeedCsv(factory); await SeedExport(factory, Guid.NewGuid()); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var files = new List<List<string[]>>();
        foreach (var (path, index) in CsvPaths(data.Supplier).Select((path, index) => (path, index)))
        {
            var response = await client.GetAsync("/api/v1/exports/" + path); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/csv", response.Content.Headers.ContentType!.MediaType); Assert.Equal("utf-8", response.Content.Headers.ContentType.CharSet);
            Assert.EndsWith(".csv", response.Content.Headers.ContentDisposition!.FileNameStar ?? response.Content.Headers.ContentDisposition.FileName!.Trim('"'));
            var bytes = await response.Content.ReadAsByteArrayAsync(); Assert.False(bytes.Take(3).SequenceEqual(new byte[] { 239, 187, 191 }));
            var text = await response.Content.ReadAsStringAsync(); Assert.DoesNotContain("FOREIGN PRIVATE", text); Assert.DoesNotContain("999", text);
            var rows = ParseCsv(text); Assert.Equal(CsvHeaders[index], rows[0]); Assert.Equal(2, rows.Count); Assert.All(rows, row => Assert.Equal(CsvHeaders[index].Length, row.Length)); files.Add(rows);
        }
        Assert.Equal("Item \"Premium\", മലയാളം\nNew", files[0][1][0]); Assert.Equal("1.2345", files[0][1][4]); Assert.Equal("", files[0][1][5]);
        Assert.Equal("0.4567", files[0][1][6]); Assert.Equal("3.1234", files[0][1][7]); Assert.Equal("", files[0][1][8]);
        Assert.Equal("critical", files[1][1][7]); Assert.Equal("ABC, Store \"മലയാളം\"", files[1][1][8]);
        Assert.Equal("", files[2][1][0]); Assert.Equal("'=DANGEROUS", files[2][1][1]); Assert.Equal("2.3456", files[2][1][3]); Assert.Equal("1.23", files[2][1][5]); Assert.Equal("", files[2][1][6]); Assert.Equal("10.50", files[2][1][7]);
        Assert.Equal("11", files[3][1][3]); Assert.Equal("11", files[4][1][5]); Assert.Equal("1", files[4][1][6]);
    }
    [Theory]
    [InlineData(Role.Owner, true)] [InlineData(Role.Manager, false)] [InlineData(Role.Staff, false)]
    public async Task CsvAccessRequiresExportRoleAndFinancialFilesRequireOwner(Role role, bool money)
    {
        using var factory = new Factory { MemberRole = role, Permission = "reports.view", RealCsvReports = true }; using var client = factory.CreateClient(); var data = await SeedCsv(factory);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); (await db.Memberships.SingleAsync()).PermissionsJson = "[\"reports.view\",\"stock.view\",\"supplier.view\",\"purchase.view\"]"; await db.SaveChangesAsync(); }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        foreach (var (path, index) in CsvPaths(data.Supplier).Select((path, index) => (path, index))) Assert.Equal(role == Role.Staff || (index >= 2 && !money) ? HttpStatusCode.Forbidden : HttpStatusCode.OK, (await client.GetAsync("/api/v1/exports/" + path)).StatusCode);
    }
    [Fact]
    public async Task AnonymousCsvRequestsAreDeniedAndManagerCannotForgeMissingStockPermission()
    {
        using var factory = new Factory { MemberRole = Role.Manager, Permission = "reports.view" }; using var client = factory.CreateClient();
        foreach (var path in CsvPaths(Guid.NewGuid())) Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/exports/" + path)).StatusCode);
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true)); Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/v1/exports/stock.csv")).StatusCode);
    }
    [Fact]
    public async Task CsvIdsCannotEscapeTenantAndDeletedItemsOrSuppliersAreUnavailable()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient(); var own = await SeedCsv(factory); var foreign = Guid.NewGuid(); await SeedExport(factory, foreign);
        Guid otherItem, otherSupplier;
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); otherItem = (await db.CatalogItems.IgnoreQueryFilters().SingleAsync(x => x.BusinessId == foreign)).Id; otherSupplier = (await db.Suppliers.IgnoreQueryFilters().SingleAsync(x => x.BusinessId == foreign)).Id; }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        foreach (var kind in new[] { "stock.csv", "low-stock.csv" }) Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/exports/{kind}?ids={otherItem}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/exports/suppliers/{otherSupplier}/purchases.csv")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/stock.csv?ids=bad-id")).StatusCode);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); (await db.CatalogItems.IgnoreQueryFilters().SingleAsync(x => x.Id == own.Item)).IsActive = false; (await db.Suppliers.IgnoreQueryFilters().SingleAsync(x => x.Id == own.Supplier)).IsActive = false; await db.SaveChangesAsync(); }
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/exports/stock.csv?ids={own.Item}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/exports/suppliers/{own.Supplier}/purchases.csv")).StatusCode);
    }
    [Fact]
    public async Task EmptyCsvResultsKeepHeadersAndInvalidRangesFailForEveryContract()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient(); Guid supplier;
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var s = new Supplier { BusinessId = BusinessId, Name = "Empty" }; db.Suppliers.Add(s); await db.SaveChangesAsync(); supplier = s.Id; }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        foreach (var (path, index) in CsvPaths(supplier).Select((path, index) => (path, index)))
        {
            Assert.Equal(CsvHeaders[index], Assert.Single(ParseCsv(await client.GetStringAsync("/api/v1/exports/" + path))));
            Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path + "?start=2026-10-02&end=2026-10-01")).StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path + "?start=2000-01-01&end=2026-10-01")).StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path + "?start=not-a-date")).StatusCode);
        }
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/stock.csv?filter=unknown")).StatusCode);
    }
    [Fact]
    public async Task UnknownReceiptUnitConversionAndMissingBagGeometryStayBlank()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient(); await SeedCsv(factory);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var line = await db.PurchaseItems.IgnoreQueryFilters().SingleAsync(); line.Unit = "KG"; await db.SaveChangesAsync(); }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true)); Assert.Equal("", ParseCsv(await client.GetStringAsync("/api/v1/exports/stock.csv"))[1][6]);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var line = await db.PurchaseItems.IgnoreQueryFilters().SingleAsync(); line.Unit = "BAG"; line.KgPerUnit = null; await db.SaveChangesAsync(); }
        Assert.Equal("", ParseCsv(await client.GetStringAsync("/api/v1/exports/reports/items.csv"))[1][1]); Assert.Equal("", ParseCsv(await client.GetStringAsync("/api/v1/exports/reports/suppliers.csv"))[1][2]);
    }
    [Fact]
    public async Task CsvRefusesOversizedPurchaseResultsInsteadOfSilentlyTruncating()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient(); await SeedExport(factory, count: 2001);
        Guid supplier; using (var scope = factory.Services.CreateScope()) supplier = (await scope.ServiceProvider.GetRequiredService<AppDbContext>().Suppliers.IgnoreQueryFilters().SingleAsync()).Id;
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        foreach (var path in CsvPaths(supplier).Skip(2)) Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path)).StatusCode);
    }
    [Fact]
    public async Task CsvRefusesOversizedCatalogResultsInBothStockFormats()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var category = new Category { BusinessId = BusinessId, Name = "Large" }; db.CatalogItems.AddRange(Enumerable.Range(0, 5001).Select(i => new CatalogItem { BusinessId = BusinessId, Category = category, Name = "Item" + i, ItemCode = "I" + i, CurrentStock = 1, ReorderLevel = 2 })); await db.SaveChangesAsync(); }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true)); foreach (var path in new[] { "stock.csv", "low-stock.csv" }) Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path)).StatusCode);
    }
    [Fact]
    public async Task CsvPackReportsKeepBoxTinAndExplicitKgNameFallbackSeparateAndCountDistinctPurchases()
    {
        using var factory = new Factory { MemberRole = Role.Owner, RealCsvReports = true }; using var client = factory.CreateClient(); var seed = await SeedCsv(factory);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var purchase = await db.Purchases.IgnoreQueryFilters().Include(x => x.Items).SingleAsync();
            var existing = await db.CatalogItems.IgnoreQueryFilters().SingleAsync();
            db.PurchaseItems.Add(new PurchaseItem { BusinessId = BusinessId, PurchaseOrderId = purchase.Id, CatalogItemId = existing.Id, Unit = "BOX", OrderedQuantity = 3, LineTotal = 20m });
            db.PurchaseItems.Add(new PurchaseItem { BusinessId = BusinessId, PurchaseOrderId = purchase.Id, CatalogItemId = existing.Id, Unit = "TIN", OrderedQuantity = 4, LineTotal = 30m });
            var sugar = new CatalogItem { BusinessId = BusinessId, CategoryId = existing.CategoryId, Name = "SUGAR 50 KG", ItemCode = "S50", DefaultUnit = "KG" }; db.CatalogItems.Add(sugar);
            db.PurchaseItems.Add(new PurchaseItem { BusinessId = BusinessId, PurchaseOrderId = purchase.Id, CatalogItem = sugar, Unit = "KG", OrderedQuantity = 100m, LineTotal = 40m });
            await db.SaveChangesAsync();
        }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var items = ParseCsv(await client.GetStringAsync("/api/v1/exports/reports/items.csv")); Assert.Equal(3, items.Count);
        var premium = items.Single(row => row[0].StartsWith("Item")); Assert.Equal("3", premium[3]); Assert.Equal("4", premium[4]); Assert.Equal("1", premium[6]); Assert.Equal("61", premium[5]);
        var sugarRow = items.Single(row => row[0] == "SUGAR 50 KG"); Assert.Equal("100", sugarRow[1]); Assert.Equal("2", sugarRow[2]); Assert.Equal("40", sugarRow[5]);
        var supplier = ParseCsv(await client.GetStringAsync("/api/v1/exports/reports/suppliers.csv")); Assert.Equal("4.35", supplier[1][1]); Assert.Equal("101", supplier[1][3]);
    }
}
