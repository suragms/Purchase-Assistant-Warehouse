using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using Xunit;

namespace PurchaseAssistant.IntegrationTests.Purchases
{
    public class PurchaseIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString =
            "Host=localhost;Database=warehouse_erp_dev;Username=modelbridge;Password=modelbridge";

        private AppDbContext _context = null!;
        private StubTenant _tenant = null!;
        private StubUser _user = null!;

        private Guid _businessId;
        private Guid _userId;
        private Guid _categoryId;
        private Guid _supplierId;
        private Guid _catalogItemId;

        public async Task InitializeAsync()
        {
            _businessId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _categoryId = Guid.NewGuid();
            _supplierId = Guid.NewGuid();
            _catalogItemId = Guid.NewGuid();

            _tenant = new StubTenant(_businessId);
            _user = new StubUser(_businessId, _userId);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
                .Options;

            _context = new AppDbContext(options, _tenant);

            // Seed Business, User, Category, Supplier, CatalogItem
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Businesses\" (\"Id\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, {1}, true, NOW())",
                _businessId, $"POIntegTest-{_businessId}");

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Users\" (\"Id\", \"Name\", \"Email\", \"PasswordHash\", \"Status\", \"CreatedAt\") VALUES ({0}, 'PO Test User', {1}, 'hash', 1, NOW())",
                _userId, $"po-integ-{_userId}@test.com");

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Categories\" (\"Id\", \"BusinessId\", \"Name\", \"CreatedAt\") VALUES ({0}, {1}, 'PO Category', NOW())",
                _categoryId, _businessId);

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Suppliers\" (\"Id\", \"BusinessId\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, {1}, 'Test Supplier', true, NOW())",
                _supplierId, _businessId);

            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO ""CatalogItems""
                  (""Id"",""BusinessId"",""CategoryId"",""Name"",""ItemCode"",""CurrentStock"",""ReservedStock"",""PhysicalStock"",""ReorderLevel"",""RowVersion"",""DefaultUnit"",""IsActive"",""CreatedAt"")
                  VALUES ({0},{1},{2},'PO Catalog Item','PITEM-01',50,0,50,0,{3},'PCS',true,NOW())",
                _catalogItemId, _businessId, _categoryId, Guid.NewGuid());
        }

        public async Task DisposeAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"PurchaseItems\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Purchases\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"StockMovements\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogItems\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Suppliers\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Categories\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\" WHERE \"Id\" = {0}", _userId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Businesses\" WHERE \"Id\" = {0}", _businessId);

            await _context.DisposeAsync();
        }

        private PurchaseService CreatePurchaseService() =>
            new PurchaseService(_context, _user, new StockService(_context, _user));

        [Fact]
        public async Task PurchaseLifecycle_CreateConfirmReceive_CommitsStockViaStockService()
        {
            var sut = CreatePurchaseService();

            // 1. Create Purchase Order (Draft)
            var createDto = new UpsertPurchaseOrderDto
            {
                OrderNumber = "PO-2026-001",
                SupplierId = _supplierId,
                Notes = "Test PO",
                TaxTotal = 10.00m,
                Items = new List<UpsertPurchaseItemDto>
                {
                    new UpsertPurchaseItemDto
                    {
                        CatalogItemId = _catalogItemId,
                        OrderedQuantity = 100m,
                        UnitPrice = 5.00m,
                        Notes = "100 units"
                    }
                }
            };

            var created = await sut.CreatePurchaseOrderAsync(createDto);
            created.Should().NotBeNull();
            created.Status.Should().Be(PurchaseStatus.Draft);
            created.Subtotal.Should().Be(500.00m);
            created.GrandTotal.Should().Be(510.00m);

            // 2. Confirm Order
            var confirmed = await sut.UpdateStatusAsync(created.Id, PurchaseStatus.Confirmed);
            confirmed.Status.Should().Be(PurchaseStatus.Confirmed);
            confirmed.ConfirmedAt.Should().NotBeNull();

            // 3. Dispatch Order
            var dispatched = await sut.UpdateStatusAsync(created.Id, PurchaseStatus.Dispatched);
            dispatched.Status.Should().Be(PurchaseStatus.Dispatched);

            // 4. Receive Items (Should trigger StockService.AdjustStockAsync and create PurchaseReceipt movement)
            var receiveDto = new ReceivePurchaseDto
            {
                Items = new List<ReceivePurchaseItemDto>
                {
                    new ReceivePurchaseItemDto
                    {
                        PurchaseItemId = created.Items[0].Id,
                        ReceivedQuantityDelta = 100m,
                        Notes = "Received full shipment"
                    }
                }
            };

            var receivedOrder = await sut.ReceiveItemsAsync(created.Id, receiveDto);
            receivedOrder.Status.Should().Be(PurchaseStatus.Completed);
            receivedOrder.DeliveryState.Should().Be(DeliveryState.Delivered);
            receivedOrder.Items[0].ReceivedQuantity.Should().Be(100m);

            // 5. Verify Stock was updated via StockService (50 original + 100 received = 150)
            var catalogItem = await _context.CatalogItems
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstAsync(c => c.Id == _catalogItemId);

            catalogItem.CurrentStock.Should().Be(150m, "stock must be increased by received purchase quantity");

            // 6. Verify StockMovement was logged with reason "PurchaseReceipt"
            var movement = await _context.StockMovements
                .AsNoTracking()
                .IgnoreQueryFilters()
                .SingleAsync(m => m.CatalogItemId == _catalogItemId);

            movement.Reason.Should().Be("PurchaseReceipt");
            movement.QuantityDelta.Should().Be(100m);
            movement.QuantityBefore.Should().Be(50m);
            movement.QuantityAfter.Should().Be(150m);
        }
    }

    internal class StubTenant : ITenantProvider
    {
        private readonly Guid _businessId;
        public StubTenant(Guid businessId) => _businessId = businessId;
        public Guid GetBusinessId() => _businessId;
    }

    internal class StubUser : ICurrentUserService
    {
        public StubUser(Guid businessId, Guid userId)
        {
            BusinessId = businessId;
            UserId = userId;
        }

        public Guid? UserId { get; }
        public string Email => "po-integ@test.com";
        public Guid? BusinessId { get; }
        public string Role => "Admin";
        public IEnumerable<string> Permissions => new[] { "purchase.view", "purchase.create", "purchase.edit", "purchase.verify", "purchase.commit", "stock.view", "stock.adjust" };
        public bool HasPermission(string permission) => true;
    }
}
