using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Web.Services;

namespace PurchaseAssistant.UnitTests.AI;
public partial class PurchaseIntentEndpointTests
{
    private static async Task SeedExport(Factory factory, Guid? tenant = null, int count = 1, bool deleted = false)
    {
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var business = tenant ?? BusinessId;
        if (tenant.HasValue) db.Businesses.Add(new Business { Id = business, Name = "OTHER TENANT PRIVATE" });
        var category = new Category { BusinessId = business, Name = "Grain" };
        var item = new CatalogItem { BusinessId = business, Category = category, Name = tenant.HasValue ? "FOREIGN PRIVATE ITEM" : "=HYPERLINK(\"private\")", ItemCode = "RICE", CurrentStock = 1.2345m, IsActive = !deleted };
        var supplier = new Supplier { BusinessId = business, Name = tenant.HasValue ? "FOREIGN PRIVATE SUPPLIER" : "Test supplier" };
        db.CatalogItems.Add(item); db.Suppliers.Add(supplier);
        for (var i = 0; i < count; i++) db.Purchases.Add(new PurchaseOrder { BusinessId = business, OrderNumber = "../unsafe/order-" + i, Supplier = supplier,
            Status = PurchaseStatus.Confirmed, CreatedAt = DateTime.UtcNow, GrandTotal = 123.4567m, PaidAmount = 23.4567m,
            Items = [new PurchaseItem { BusinessId = business, CatalogItem = item, OrderedQuantity = 2.3456m, ReceivedQuantity = 1.2345m, UnitPrice = 52.6321m, LineTotal = 123.4567m }] });
        await db.SaveChangesAsync();
    }
    [Theory]
    [InlineData(Role.Owner)] [InlineData(Role.SuperAdmin)] [InlineData(Role.Manager)] [InlineData(Role.Admin)]
    public async Task ExportAllFormatsAreScopedAndMoneyIsOwnerOrSuperAdminOnly(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = "reports.view" }; using var client = factory.CreateClient();
        await SeedExport(factory); await SeedExport(factory, Guid.NewGuid()); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var json = await client.GetAsync("/api/v1/exports/backup.json"); Assert.Equal(HttpStatusCode.OK, json.StatusCode);
        var body = await json.Content.ReadAsStringAsync(); Assert.DoesNotContain("FOREIGN PRIVATE", body);
        var seesMoney = role is Role.Owner or Role.SuperAdmin;
        Assert.Equal(seesMoney, body.Contains("grandTotal")); Assert.Equal(seesMoney, body.Contains("unitPrice"));
        using var data = JsonDocument.Parse(body); Assert.Equal(1, data.RootElement.GetProperty("purchases").GetArrayLength());
        Assert.Equal(1.2345m, data.RootElement.GetProperty("stock")[0].GetProperty("CurrentStock").GetDecimal());
        var xlsx = await client.GetAsync("/api/v1/exports/stock.xlsx"); Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", xlsx.Content.Headers.ContentType!.MediaType);
        Assert.Contains("harisree_stock_", xlsx.Content.Headers.ContentDisposition!.FileName); Assert.Contains("no-store", xlsx.Headers.CacheControl!.ToString());
        using (var zip = new ZipArchive(new MemoryStream(await xlsx.Content.ReadAsByteArrayAsync()))) { using var reader = new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml")!.Open()); var sheet = await reader.ReadToEndAsync(); Assert.Contains("1.2345", sheet); Assert.DoesNotContain("<f>", sheet); Assert.DoesNotContain("FOREIGN PRIVATE", sheet); }
        var pdf = await client.GetAsync("/api/v1/exports/purchases.pdf"); Assert.Equal("application/pdf", pdf.Content.Headers.ContentType!.MediaType); Assert.StartsWith("%PDF", System.Text.Encoding.ASCII.GetString(await pdf.Content.ReadAsByteArrayAsync()));
        var archive = await client.PostAsJsonAsync("/api/v1/exports/backup", new { rangePreset = "quarter" }); Assert.Equal(HttpStatusCode.OK, archive.StatusCode);
        using var pack = new ZipArchive(new MemoryStream(await archive.Content.ReadAsByteArrayAsync()));
        Assert.NotNull(pack.GetEntry("README.txt")); Assert.NotNull(pack.GetEntry("Summary.txt")); Assert.NotNull(pack.GetEntry("purchases_summary.pdf"));
        Assert.All(pack.Entries, e => Assert.DoesNotContain("..", e.FullName)); Assert.Single(pack.Entries, e => e.FullName.StartsWith("orders/"));
        using var summary = new StreamReader(pack.GetEntry("Summary.txt")!.Open()); var totals = await summary.ReadToEndAsync(); Assert.Equal(seesMoney, totals.Contains("Total INR")); if (seesMoney) Assert.Contains("123.46", totals); // Reference display precision; stored JSON above retains four decimals.
        using var auditScope = factory.Services.CreateScope(); var audit = await auditScope.ServiceProvider.GetRequiredService<AppDbContext>().SecurityAuditLogs.IgnoreQueryFilters().Where(x => x.EventType == "business_export").ToListAsync();
        Assert.Equal(4, audit.Count); Assert.All(audit, x => { Assert.Equal(BusinessId, x.BusinessId); Assert.Equal(UserId, x.UserId); Assert.DoesNotContain("FOREIGN", x.MetadataJson); Assert.DoesNotContain("123.4567", x.MetadataJson); });
    }
    [Theory]
    [InlineData("/api/v1/exports/stock.xlsx")] [InlineData("/api/v1/exports/backup.json")] [InlineData("/api/v1/exports/backup/logs")]
    public async Task ExportAndHistoryRequireAuthenticationAndRole(string path)
    {
        using var factory = new Factory { MemberRole = Role.Staff, Permission = "reports.view" }; using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync(path)).StatusCode);
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true)); Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsync("/api/v1/exports/backup/run", null)).StatusCode);
    }
    [Theory]
    [InlineData("?start=invalid")] [InlineData("?start=2026-02-01&end=2026-01-01")] [InlineData("?start=2000-01-01&end=2026-01-01")]
    public async Task ExportRejectsMalformedOrOversizedRanges(string query)
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        foreach (var path in new[] { "purchases.pdf", "backup.json", "backup.zip" }) Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/exports/" + path + query)).StatusCode);
    }
    [Fact]
    public async Task ExportPresetsRejectUnknownAndZipBoundsAreEnforced()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/exports/backup", new { rangePreset = "../private" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/v1/exports/backup", new { rangePreset = "all" })).StatusCode);
        await SeedExport(factory, count: 401);
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, (await client.GetAsync("/api/v1/exports/backup.zip")).StatusCode);
    }
    [Fact]
    public async Task DefaultJsonRangeExcludesOldDraftCancelledAndInactiveCatalog()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); await SeedExport(factory, count: 4, deleted: true);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var orders = await db.Purchases.IgnoreQueryFilters().ToListAsync(); orders[0].CreatedAt = DateTime.UtcNow.AddDays(-100); orders[1].Status = PurchaseStatus.Draft; orders[2].Status = PurchaseStatus.Cancelled; await db.SaveChangesAsync(); }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true)); using var data = JsonDocument.Parse(await client.GetStringAsync("/api/v1/exports/backup.json"));
        Assert.Equal(1, data.RootElement.GetProperty("purchases").GetArrayLength()); Assert.Equal(0, data.RootElement.GetProperty("stock").GetArrayLength());
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/businesses/{Guid.NewGuid()}/exports/backup.json")).StatusCode);
    }
    [Theory]
    [InlineData(Role.Owner)] [InlineData(Role.Manager)]
    public async Task StoredBackupIsBusinessJsonWithoutSecretsOrMoneyAndHasPrivateHistory(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = "reports.view" }; using var client = factory.CreateClient(); await SeedExport(factory); var foreign = Guid.NewGuid(); await SeedExport(factory, foreign);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.BackupLogs.Add(new() { BusinessId = foreign, Status = "success", FilePath = "../PRIVATE-FILE" }); await db.SaveChangesAsync(); }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true)); var run = await client.PostAsync("/api/v1/exports/backup/run", null); Assert.Equal(HttpStatusCode.OK, run.StatusCode);
        var log = await run.Content.ReadFromJsonAsync<BackupLogDto>(); Assert.NotNull(log); Assert.Equal("success", log!.Status); Assert.DoesNotContain("/", log.FilePath!); Assert.DoesNotContain("\\", log.FilePath!);
        var file = await File.ReadAllTextAsync(Path.Combine(factory.BackupDirectory, BusinessId.ToString(), log.FilePath!));
        using var data = JsonDocument.Parse(file); Assert.Equal(BusinessBackupService.SchemaVersion, data.RootElement.GetProperty("schema_version").GetString());
        Assert.DoesNotContain("FOREIGN PRIVATE", file); Assert.DoesNotContain("Password", file); Assert.DoesNotContain("Token", file); Assert.DoesNotContain("grandTotal", file); Assert.DoesNotContain("unitPrice", file);
        Assert.Equal(1, data.RootElement.GetProperty("purchases").GetArrayLength()); Assert.Equal(1.2345m, data.RootElement.GetProperty("catalog")[0].GetProperty("current_stock").GetDecimal());
        using var history = JsonDocument.Parse(await client.GetStringAsync("/api/v1/exports/backup/logs")); Assert.Equal(1, history.RootElement.GetProperty("items").GetArrayLength());
        using var auditScope = factory.Services.CreateScope(); Assert.Single(await auditScope.ServiceProvider.GetRequiredService<AppDbContext>().SecurityAuditLogs.IgnoreQueryFilters().Where(x => x.EventType == "backup_run" && x.BusinessId == BusinessId).ToListAsync());
        var validate = await client.PostAsJsonAsync("/api/v1/exports/restore/dry-run", new { payload = data.RootElement }); Assert.Equal(role == Role.Owner ? HttpStatusCode.OK : HttpStatusCode.Forbidden, validate.StatusCode);
        if (role == Role.Owner) Assert.True(JsonDocument.Parse(await validate.Content.ReadAsStringAsync()).RootElement.GetProperty("valid").GetBoolean());
        Assert.Equal(role == Role.Owner ? HttpStatusCode.NotImplemented : HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/exports/restore/commit", new {})).StatusCode);
    }
    [Fact]
    public async Task BackupStorageFailureReturnsSafeRecordedMetadata()
    {
        using var factory = new Factory { MemberRole = Role.Owner, BackupDirectory = "\0invalid-private-secret-root" }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var response = await client.PostAsync("/api/v1/exports/backup/run", null); Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.DoesNotContain("private-secret-root", await response.Content.ReadAsStringAsync()); Assert.Contains("could not be completed", await response.Content.ReadAsStringAsync());
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); Assert.Equal("fail", (await db.BackupLogs.IgnoreQueryFilters().SingleAsync()).Status);
    }
    [Theory]
    [InlineData("{\"businessId\":{},\"schemaVersion\":1}")]
    [InlineData("{\"businessId\":\"not-a-guid\",\"schemaVersion\":\"private-value\"}")]
    [InlineData("{\"payload\":[]}")]
    public async Task MalformedRestoreNeverWritesOrLeaksInternalErrors(string input)
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var response = await client.PostAsync("/api/v1/exports/restore/dry-run", new StringContent(input, System.Text.Encoding.UTF8, "application/json"));
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.BadRequest); Assert.DoesNotContain("Exception", await response.Content.ReadAsStringAsync());
        using var scope = factory.Services.CreateScope(); Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().BackupLogs.IgnoreQueryFilters().ToListAsync());
    }
    [Fact]
    public void NightlyBackupUsesTwoAmIndiaTimeAcrossDayBoundary()
    {
        Assert.Equal(DateTimeOffset.Parse("2026-10-02T20:30:00Z"), BusinessBackupWorker.NextRun(DateTimeOffset.Parse("2026-10-02T20:29:59Z")));
        Assert.Equal(DateTimeOffset.Parse("2026-10-03T20:30:00Z"), BusinessBackupWorker.NextRun(DateTimeOffset.Parse("2026-10-02T20:30:00Z")));
    }
    [Fact]
    public async Task OwnerTelemetryCountsOnlyTodayAndSelectedBusinessAndHistoryIsBounded()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); await SeedExport(factory, Guid.NewGuid());
        using (var scope = factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var foreign = await db.Businesses.FirstAsync(x => x.Id != BusinessId);
            db.AiUsageLogs.AddRange(new AiUsageLog { BusinessId = BusinessId }, new AiUsageLog { BusinessId = BusinessId, CreatedAt = DateTime.UtcNow.Date.AddDays(-1) }, new AiUsageLog { BusinessId = foreign.Id });
            for (var i = 0; i < 55; i++) db.BackupLogs.Add(new BackupLog { BusinessId = BusinessId, Status = "success", CreatedAt = DateTime.UtcNow.AddMinutes(-i), FilePath = "../../PRIVATE" });
            await db.SaveChangesAsync();
        }
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        using var dashboard = JsonDocument.Parse(await client.GetStringAsync("/api/v1/operations/owner-dashboard")); Assert.Equal(1, dashboard.RootElement.GetProperty("aiRequestsToday").GetInt32()); Assert.Equal("success", dashboard.RootElement.GetProperty("backupLastStatus").GetString());
        var history = await client.GetStringAsync("/api/v1/exports/backup/logs"); Assert.DoesNotContain("PRIVATE", history); using var parsed = JsonDocument.Parse(history); Assert.Equal(50, parsed.RootElement.GetProperty("items").GetArrayLength());
    }
}
