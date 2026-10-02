using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Services;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.IntegrationTests.Stock;
public partial class StockServiceIntegrationTests
{
    private const string HistoricalKey = "synthetic-preview-test-signing-key-32-bytes";
    private sealed class HistoricalHost : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); builder.UseSetting("Jwt:SecretKey", HistoricalKey);
            builder.ConfigureServices(services => {
                services.RemoveAll<DbContextOptions<AppDbContext>>(); services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(o => o.UseNpgsql(ConnectionString));
            });
        }
    }
    private sealed class HistoricalUser(Guid business, Guid actor, string role) : ICurrentUserService
    {
        public Guid? BusinessId => business; public Guid? UserId => actor; public string Email => "synthetic@test.local";
        public string Role => role; public IEnumerable<string> Permissions => ["reports.view", "catalog.edit", "purchase.edit", "purchase.view"];
        public bool HasPermission(string permission) => true;
    }
    private async Task SeedHistoricalRelatedAsync()
    {
        (await _context.Users.SingleAsync(x => x.Id == _userId)).Status = UserStatus.Active;
        var data = await SeedCsvPg("BAG", 2.5m);
        _context.SupplierItems.Add(new SupplierItem { BusinessId = _businessId, SupplierId = data.Supplier.Id, CatalogItemId = data.Item.Id, SupplierItemCode = "synthetic" });
        _context.SupplierItemPrices.Add(new SupplierItemPrice { BusinessId = _businessId, SupplierId = data.Supplier.Id, CatalogItemId = data.Item.Id, Unit = "BAG", Price = 123.45m });
        _context.SecurityAuditLogs.Add(new SecurityAuditLog { BusinessId = _businessId, UserId = _userId, EventType = "synthetic-baseline", Description = "Must stay unchanged" });
        _context.BackupLogs.Add(new BackupLog { BusinessId = _businessId, Status = "synthetic-baseline" });
        _context.AiUsageLogs.Add(new AiUsageLog { BusinessId = _businessId, Feature = "synthetic-baseline" });
        await _context.SaveChangesAsync();
        // A nonempty immutable ledger is included in the before/after proof.
        await new StockService(_context, _user).AdjustStockAsync(data.Item.Id, new PurchaseAssistant.Application.DTOs.Stock.AdjustStockRequestDto { QuantityDelta = 1m, Reason = "synthetic baseline", ExpectedVersion = data.Item.RowVersion });
        _context.ChangeTracker.Clear();
    }
    private async Task<IReadOnlyList<HistoricalTarget>> HistoricalTargetsAsync() => await _context.PurchaseItems.AsNoTracking()
        .Select(x => new HistoricalTarget(x.BusinessId, x.CatalogItemId, x.CatalogItem.ItemCode, x.CatalogItem.Barcode,
            x.PurchaseOrderId, x.Id, x.PurchaseOrder.SupplierId, x.CatalogItem.Name, x.CatalogItem.DefaultUnit, x.Unit,
            x.CatalogItem.CurrentStock, x.CatalogItem.PhysicalStock, x.KgPerUnit)).ToListAsync();
    private HistoricalPreviewInput MapSyntheticRow(HistoricalPreviewInput input, HistoricalTarget target)
    {
        var r = input.Rows[0] with { ItemId = target.ItemId, ItemCode = target.ItemCode, Barcode = target.Barcode,
            PurchaseId = target.PurchaseId, LineId = target.LineId, SupplierId = target.SupplierId };
        return input with { Rows = [r], SourceMap = [input.SourceMap[0] with { ItemId = target.ItemId }] };
    }
    // Every column and row (including embedded Unit fields and xmin versions), not merely selected counters.
    private async Task<Dictionary<string, string>> HistoricalDatabaseSnapshotAsync()
    {
        var result = new Dictionary<string, string>();
        await using var connection = new NpgsqlConnection(ConnectionString); await connection.OpenAsync();
        var tables = new List<string>();
        await using (var command = new NpgsqlCommand("SELECT table_name FROM information_schema.columns WHERE table_schema='public' AND column_name='BusinessId' ORDER BY table_name", connection))
        await using (var reader = await command.ExecuteReaderAsync()) while (await reader.ReadAsync()) tables.Add(reader.GetString(0));
        foreach (var table in tables)
        {
            // Table names come from PostgreSQL metadata, quoted as identifiers; tenant is always a parameter.
            var quoted = '"' + table.Replace("\"", "\"\"") + '"';
            await using var command = new NpgsqlCommand($"SELECT COALESCE(jsonb_agg(to_jsonb(t) || jsonb_build_object('xmin', t.xmin::text) ORDER BY t.\"Id\"), '[]'::jsonb)::text FROM {quoted} t WHERE t.\"BusinessId\"=@business", connection);
            command.Parameters.AddWithValue("business", _businessId); result[table] = (string)(await command.ExecuteScalarAsync())!;
        }
        Assert.Contains("CatalogItems", result.Keys); Assert.Contains("StockMovements", result.Keys); Assert.Contains("SupplierItems", result.Keys);
        Assert.Contains("SupplierItemPrices", result.Keys); Assert.Contains("PurchaseItems", result.Keys); Assert.Contains("Purchases", result.Keys);
        Assert.Contains("SecurityAuditLogs", result.Keys); Assert.Contains("BackupLogs", result.Keys); Assert.Contains("AiUsageLogs", result.Keys);
        return result;
    }
    private static void EqualSnapshots(Dictionary<string, string> before, Dictionary<string, string> after)
    { Assert.Equal(before.Keys, after.Keys); foreach (var table in before.Keys) Assert.Equal(before[table], after[table]); }

    [Fact]
    public async Task HistoricalPreviewMatchesRealPostgresEntitiesAndCommitsReadOnlyTransactionWithEveryColumnUnchanged()
    {
        await SeedHistoricalRelatedAsync(); var before = await HistoricalDatabaseSnapshotAsync();
        var data = HistoricalPreviewFixtures.Create("valid", _businessId, _userId);
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            await _context.Database.ExecuteSqlRawAsync("SET TRANSACTION READ ONLY");
            var targets = await HistoricalTargetsAsync(); var target = Assert.Single(targets);
            var result = HistoricalMetadataValidator.Preview(MapSyntheticRow(data.Input, target), targets, new HistoricalUser(_businessId, _userId, "Owner"));
            Assert.Equal("VALID", Assert.Single(result.Rows).Outcome); Assert.Equal(target.ItemId, result.Rows[0].MatchedItemId);
            Assert.False(result.WritesPerformed); Assert.False(_context.ChangeTracker.HasChanges());
            await transaction.CommitAsync(); // No rollback masking possible writes.
        }
        EqualSnapshots(before, await HistoricalDatabaseSnapshotAsync());
    }
    [Theory, InlineData(Role.Owner, 200), InlineData(Role.SuperAdmin, 200), InlineData(Role.Manager, 403), InlineData(Role.Staff, 403)]
    public async Task HistoricalPreviewHttpUsesPostgresMembershipAndMakesNoDurableWrites(Role role, int status)
    {
        await SeedHistoricalRelatedAsync(); var session = Guid.NewGuid();
        _context.Memberships.Add(new Membership { BusinessId = _businessId, UserId = _userId, Role = role, PermissionsJson = "[\"reports.view\",\"catalog.edit\",\"purchase.edit\",\"purchase.view\"]" });
        _context.RefreshTokens.Add(new RefreshToken { UserId = _userId, FamilyId = session, TokenHash = "synthetic-only", TokenDigest = new string('0', 64), ExpiresAt = DateTime.UtcNow.AddHours(1) });
        await _context.SaveChangesAsync(); _context.ChangeTracker.Clear(); var before = await HistoricalDatabaseSnapshotAsync();
        using var host = new HistoricalHost(); using var client = host.CreateClient();
        string Token(Guid business) => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken("PurchaseAssistant", "PurchaseAssistantApp",
            [new(ClaimTypes.NameIdentifier, _userId.ToString()), new("businessId", business.ToString()), new("sessionId", session.ToString()), new("role", "Owner"), new("permissions", "reports.view")],
            expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(HistoricalKey)), SecurityAlgorithms.HmacSha256)));
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(_businessId));
        foreach (var suite in HistoricalPreviewFixtures.Suites)
            Assert.Equal(status, (int)(await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = suite })).StatusCode);
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "valid" })).StatusCode);
        EqualSnapshots(before, await HistoricalDatabaseSnapshotAsync());
    }
    [Fact]
    public async Task HistoricalPreviewRejectsActualForeignPostgresItemSupplierAndSourceMapsWithoutLeaksOrWrites()
    {
        await SeedHistoricalRelatedAsync(); var foreign = new StockServiceIntegrationTests(); await foreign.InitializeAsync();
        try
        {
            await foreign.SeedHistoricalRelatedAsync(); var before = await HistoricalDatabaseSnapshotAsync(); var foreignBefore = await foreign.HistoricalDatabaseSnapshotAsync();
            var own = Assert.Single(await HistoricalTargetsAsync()); var other = Assert.Single(await foreign.HistoricalTargetsAsync());
            var data = HistoricalPreviewFixtures.Create("valid", _businessId, _userId); var input = MapSyntheticRow(data.Input, own);
            foreach (var row in new[] { input.Rows[0] with { ItemId = other.ItemId }, input.Rows[0] with { SupplierId = other.SupplierId }, input.Rows[0] with { BusinessId = other.BusinessId } })
            {
                var result = HistoricalMetadataValidator.Preview(input with { Rows = [row] }, [own, other], new HistoricalUser(_businessId, _userId, "Owner"));
                Assert.Equal("OUT_OF_SCOPE", result.Rows[0].Match); Assert.Equal("REJECTED", result.Rows[0].Outcome); Assert.Empty(result.Rows[0].UnchangedCurrentValues);
            }
            var badMap = input.SourceMap[0] with { BusinessId = other.BusinessId, ItemId = other.ItemId };
            Assert.Equal("OUT_OF_SCOPE", HistoricalMetadataValidator.Preview(input with { SourceMap = [badMap] }, [own, other], new HistoricalUser(_businessId, _userId, "Owner")).Rows[0].Match);
            EqualSnapshots(before, await HistoricalDatabaseSnapshotAsync()); EqualSnapshots(foreignBefore, await foreign.HistoricalDatabaseSnapshotAsync());
        }
        finally { await foreign.DisposeAsync(); }
    }
}
