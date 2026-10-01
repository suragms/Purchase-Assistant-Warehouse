using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using Xunit;

namespace PurchaseAssistant.IntegrationTests.Dashboard
{
    public class DashboardIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Host=localhost;Database=warehouse_erp_dev;Username=modelbridge;Password=modelbridge";

        private AppDbContext _context = null!;
        private IDashboardService _service = null!;
        private Guid _businessId;
        private Guid _userId;
        private Guid? _otherBusinessId;
        private class StubTenant : ITenantProvider { public Guid BId; public Guid GetBusinessId() => BId; }
        private StubTenant _tenant = null!;

        public async Task InitializeAsync()
        {
            _businessId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _tenant = new StubTenant { BId = _businessId };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
                .Options;

            _context = new AppDbContext(options, _tenant);
            _service = new DashboardService(_context);

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Businesses\" (\"Id\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, 'Dash Biz', true, NOW())",
                _businessId);

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Users\" (\"Id\", \"Name\", \"Email\", \"PasswordHash\", \"Status\", \"CreatedAt\") VALUES ({0}, 'TUser', 'tu@t.com', 'h', 1, NOW())",
                _userId);
        }

        public async Task DisposeAsync()
        {
            if (_otherBusinessId.HasValue)
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogItems\" WHERE \"BusinessId\" = {0}", _otherBusinessId.Value);
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Categories\" WHERE \"BusinessId\" = {0}", _otherBusinessId.Value);
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Businesses\" WHERE \"Id\" = {0}", _otherBusinessId.Value);
            }
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"PurchaseItems\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Purchases\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogItems\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Suppliers\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Categories\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\" WHERE \"Id\" = {0}", _userId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Businesses\" WHERE \"Id\" = {0}", _businessId);
        }

        [Fact]
        public async Task GetDashboardData_ShouldAggregateCorrectMetrics_And_EnforceTenantIsolation()
        {
            var categoryId = Guid.NewGuid();
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Categories\" (\"Id\", \"BusinessId\", \"Name\", \"CreatedAt\") VALUES ({0}, {1}, 'Cat 1', NOW())",
                categoryId, _businessId);

            var supplierId = Guid.NewGuid();
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Suppliers\" (\"Id\", \"BusinessId\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, {1}, 'Sup 1', true, NOW())",
                supplierId, _businessId);

            // Add 1 Normal Item, 1 Low Stock Item, 1 Out of Stock Item
            await AddCatalogItem(Guid.NewGuid(), 50, 10);
            await AddCatalogItem(Guid.NewGuid(), 5, 10); // Low
            await AddCatalogItem(Guid.NewGuid(), 0, 10); // Out

            // Add 1 Other tenant item (Out of Stock), should NOT be counted
            var otherBId = Guid.NewGuid();
            _otherBusinessId = otherBId;
            var otherCategoryId = Guid.NewGuid();
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO \"Businesses\" (\"Id\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, 'Other dashboard fixture', true, NOW())", otherBId);
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO \"Categories\" (\"Id\", \"BusinessId\", \"Name\", \"CreatedAt\") VALUES ({0}, {1}, 'Other category', NOW())", otherCategoryId, otherBId);
            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO ""CatalogItems"" (""Id"",""BusinessId"",""CategoryId"",""Name"",""ItemCode"",""CurrentStock"",""ReservedStock"",""PhysicalStock"",""ReorderLevel"",""RowVersion"",""DefaultUnit"",""IsActive"",""CreatedAt"")
                  VALUES (gen_random_uuid(),{0},{1},'OItem','O1',0,0,0,10,gen_random_uuid(),'PCS',true,NOW())",
                otherBId, otherCategoryId);

            // Add purchases
            await AddPurchase(Guid.NewGuid(), supplierId, PurchaseStatus.Draft, 100);
            await AddPurchase(Guid.NewGuid(), supplierId, PurchaseStatus.Confirmed, 200);
            await AddPurchase(Guid.NewGuid(), supplierId, PurchaseStatus.Completed, 500);

            var dash = await _service.GetDashboardDataAsync();

            dash.StockMetrics.TotalCatalogItems.Should().Be(3);
            dash.StockMetrics.LowStockCount.Should().Be(1);
            dash.StockMetrics.OutOfStockCount.Should().Be(1);

            dash.PurchaseMetrics.TodayPurchasesCount.Should().Be(3);
            dash.PurchaseMetrics.PendingPurchasesCount.Should().Be(2); // Draft + Confirmed
            dash.PurchaseMetrics.ActivePurchasesCount.Should().Be(2);
            dash.PurchaseMetrics.CompletedPurchasesCount.Should().Be(1);
            dash.PurchaseMetrics.TotalPurchaseSpend.Should().Be(700);

            // Alerts
            dash.OperationalAlerts.Should().Contain(a => a.Type == NotificationType.OutOfStock);
            dash.OperationalAlerts.Should().Contain(a => a.Type == NotificationType.LowStock);

            // Clean up other tenant item
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogItems\" WHERE \"BusinessId\" = {0}", otherBId);
        }

        private async Task AddCatalogItem(Guid id, decimal stock, decimal reorder)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO ""CatalogItems"" (""Id"",""BusinessId"",""CategoryId"",""Name"",""ItemCode"",""CurrentStock"",""ReservedStock"",""PhysicalStock"",""ReorderLevel"",""RowVersion"",""DefaultUnit"",""IsActive"",""CreatedAt"")
                  VALUES ({0},{1},(SELECT ""Id"" FROM ""Categories"" WHERE ""BusinessId"" = {1} LIMIT 1),'Item','IT-' || {0}::text, {2},0,{2},{3},gen_random_uuid(),'PCS',true,NOW())",
                id, _businessId, stock, reorder);
        }

        private async Task AddPurchase(Guid id, Guid supId, PurchaseStatus status, decimal total)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO ""Purchases"" (""Id"",""BusinessId"",""OrderNumber"",""SupplierId"",""Status"",""PaymentState"",""DeliveryState"",""Subtotal"",""TaxTotal"",""GrandTotal"",""CreatedAt"")
                  VALUES ({0},{1},'PO-' || SUBSTRING({0}::text FROM 1 FOR 8),{2},{3},0,0,{4},0,{4},NOW())",
                id, _businessId, supId, (int)status, total);
        }
    }
}
