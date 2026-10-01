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
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"CatalogVariants\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"PurchaseItems\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"SecurityAuditLogs\" WHERE \"BusinessId\" = {0}", _businessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"RefreshTokens\" WHERE \"UserId\" = {0}", _userId);
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
        public async Task PostgreSqlVariantDuplicateIndexRejectsConcurrentCaseInsensitiveNames()
        {
            var service = new CatalogService(_context, new EntityNormalizationService(), _user);
            await service.CreateVariantAsync(_catalogItemId, new() { Name = "Bag", KgPerUnit = 25 });
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, _tenant);
            other.CatalogVariants.Add(new CatalogVariant { BusinessId = _businessId, CatalogItemId = _catalogItemId, Name = " BAG " });
            await FluentActions.Awaiting(() => other.SaveChangesAsync()).Should().ThrowAsync<DbUpdateException>();
            (await _context.CatalogVariants.CountAsync()).Should().Be(1);
            (await _context.StockMovements.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task PostgreSqlVariantConcurrentUpdateKeepsWinningVersion()
        {
            var service = new CatalogService(_context, new EntityNormalizationService(), _user);
            var initial = await service.CreateVariantAsync(_catalogItemId, new() { Name = "Bag", KgPerUnit = 25 });
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, _tenant);
            await other.CatalogVariants.SingleAsync(v => v.Id == initial.Id);
            var winner = await service.UpdateVariantAsync(_catalogItemId, initial.Id, new() { Name = "Winner", KgPerUnit = 10, RowVersion = initial.RowVersion });
            var loser = new CatalogService(other, new EntityNormalizationService(), _user);
            await FluentActions.Awaiting(() => loser.UpdateVariantAsync(_catalogItemId, initial.Id, new() { Name = "Loser", RowVersion = initial.RowVersion }))
                .Should().ThrowAsync<InvalidOperationException>().WithMessage("VARIANT_VERSION_CONFLICT");
            var actual = await _context.CatalogVariants.AsNoTracking().SingleAsync(v => v.Id == initial.Id);
            actual.Name.Should().Be("Winner"); actual.RowVersion.Should().Be(winner.RowVersion);
        }

        private UpsertPurchaseOrderDto NewOrder() => new()
        {
            SupplierId = _supplierId,
            Items = [new() { CatalogItemId = _catalogItemId, OrderedQuantity = 10, UnitPrice = 5 }]
        };

        [Fact]
        public async Task PostgreSqlPaymentPersistsAndRejectsReplayAndConcurrentReplacement()
        {
            var service = CreatePurchaseService(); var input = NewOrder(); input.PaymentDays = 7;
            var draft = await service.CreatePurchaseOrderAsync(input);
            var confirmed = await service.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed, draft.Version);
            var owner = new PaymentOwner(_businessId, _userId);
            var payments = new PurchaseService(_context, owner, new StockService(_context, owner));
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, _tenant);
            await other.Purchases.Include(p => p.Items).SingleAsync(p => p.Id == draft.Id);
            var paid = await payments.UpdatePaymentAsync(draft.Id, new() { PaidAmount = 12.5m, ExpectedVersion = confirmed.Version });
            paid.PaymentState.Should().Be(PaymentState.Partial); paid.RemainingAmount.Should().Be(37.5m);
            paid.Status.Should().Be(PurchaseStatus.Confirmed); paid.Version.Should().NotBe(confirmed.Version);
            await FluentActions.Awaiting(() => payments.UpdatePaymentAsync(draft.Id, new() { PaidAmount = 12.5m, ExpectedVersion = confirmed.Version }))
                .Should().ThrowAsync<InvalidOperationException>().WithMessage("PURCHASE_VERSION_CONFLICT");
            var concurrent = new PurchaseService(other, owner, new StockService(other, owner));
            await FluentActions.Awaiting(() => concurrent.UpdatePaymentAsync(draft.Id, new() { PaidAmount = 20, ExpectedVersion = confirmed.Version }))
                .Should().ThrowAsync<DbUpdateConcurrencyException>();
            _context.ChangeTracker.Clear();
            var actual = await payments.GetPurchaseOrderByIdAsync(draft.Id); actual.PaidAmount.Should().Be(12.5m);
            (await _context.StockMovements.CountAsync()).Should().Be(0);
            (await payments.GetActivityAsync(draft.Id)).Count(a => a.EventType == "PurchasePaymentUpdated").Should().Be(1);
        }

        private sealed class PaymentOwner(Guid businessId, Guid userId) : ICurrentUserService
        {
            public Guid? UserId => userId;
            public Guid? BusinessId => businessId;
            public string Role => "Owner";
            public string Email => "payment-owner@test.local";
            public IEnumerable<string> Permissions => [];
            public bool HasPermission(string permission) => true;
        }

        [Fact]
        public async Task PostgreSqlRefreshRotationIsAtomicAcrossConcurrentContexts()
        {
            var token = new RefreshToken { UserId = _userId, TokenHash = "test-original-hash", ExpiresAt = DateTime.UtcNow.AddDays(1) };
            _context.RefreshTokens.Add(token); await _context.SaveChangesAsync();
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, _tenant);
            var stale = await other.RefreshTokens.SingleAsync(t => t.Id == token.Id);
            token.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Add(new RefreshToken { UserId = _userId, FamilyId = token.FamilyId, TokenHash = "test-new-hash", ExpiresAt = DateTime.UtcNow.AddDays(1) });
            await _context.SaveChangesAsync();
            stale.RevokedAt = DateTime.UtcNow;
            other.RefreshTokens.Add(new RefreshToken { UserId = _userId, FamilyId = token.FamilyId, TokenHash = "test-losing-hash", ExpiresAt = DateTime.UtcNow.AddDays(1) });
            await FluentActions.Awaiting(() => other.SaveChangesAsync()).Should().ThrowAsync<DbUpdateConcurrencyException>();
            _context.ChangeTracker.Clear();
            (await _context.RefreshTokens.CountAsync(t => t.UserId == _userId && t.RevokedAt == null)).Should().Be(1);
            (await _context.RefreshTokens.CountAsync(t => t.UserId == _userId)).Should().Be(2);
            (await _context.RefreshTokens.Where(t => t.UserId == _userId).Select(t => t.FamilyId).Distinct().CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task PostgreSqlRejectsDuplicateRefreshDigest()
        {
            var digest = new string('A', 64);
            _context.RefreshTokens.Add(new RefreshToken { UserId = _userId, TokenHash = "first", TokenDigest = digest, ExpiresAt = DateTime.UtcNow.AddDays(1) });
            await _context.SaveChangesAsync();
            _context.RefreshTokens.Add(new RefreshToken { UserId = _userId, TokenHash = "duplicate", TokenDigest = digest, ExpiresAt = DateTime.UtcNow.AddDays(1) });
            var error = await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
            Assert.Equal("23505", Assert.IsType<Npgsql.PostgresException>(error.InnerException).SqlState);
            _context.ChangeTracker.Clear();
            Assert.Equal(1, await _context.RefreshTokens.CountAsync(t => t.UserId == _userId));
        }

        [Theory]
        [InlineData("category-type")]
        [InlineData("catalog-category")]
        [InlineData("catalog-type")]
        [InlineData("catalog-type-category-mismatch")]
        [InlineData("catalog-last-supplier")]
        [InlineData("catalog-last-broker")]
        [InlineData("variant")]
        [InlineData("supplier-item-supplier")]
        [InlineData("supplier-item-catalog")]
        [InlineData("broker-link-broker")]
        [InlineData("broker-link-supplier")]
        [InlineData("price-supplier")]
        [InlineData("price-catalog")]
        [InlineData("price-source-purchase")]
        [InlineData("purchase-supplier")]
        [InlineData("purchase-broker")]
        [InlineData("line-purchase")]
        [InlineData("line-catalog")]
        [InlineData("stock-catalog")]
        public async Task PostgreSqlRejectsCrossTenantAndWrongCategoryRelationshipsEvenForDirectWrites(string relation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var foreign = Guid.NewGuid();
            var foreignCategory = new Category { BusinessId = foreign, Name = "Other" };
            var foreignSupplier = new Supplier { BusinessId = foreign, Name = "Other supplier" };
            var foreignBroker = new Broker { BusinessId = foreign, Name = "Other broker" };
            var ownBroker = new Broker { BusinessId = _businessId, Name = "Own broker" };
            var foreignType = new CategoryType { BusinessId = foreign, CategoryId = foreignCategory.Id, Name = "Foreign type" };
            var wrongCategory = new Category { BusinessId = _businessId, Name = "Wrong category" };
            var wrongType = new CategoryType { BusinessId = _businessId, CategoryId = wrongCategory.Id, Name = "Wrong type" };
            var foreignCatalog = new CatalogItem { BusinessId = foreign, CategoryId = foreignCategory.Id, ItemCode = "FOREIGN", Name = "Other item" };
            var foreignOrder = new PurchaseOrder { BusinessId = foreign, SupplierId = foreignSupplier.Id, OrderNumber = "FOREIGN" };
            var ownOrder = new PurchaseOrder { BusinessId = _businessId, SupplierId = _supplierId, OrderNumber = "OWN" };
            _context.AddRange(new Business { Id = foreign, Name = "Other business" }, foreignCategory, foreignSupplier, foreignBroker,
                ownBroker, foreignType, wrongCategory, wrongType, foreignCatalog, foreignOrder, ownOrder);
            await _context.SaveChangesAsync();
            var catalog = new CatalogItem { BusinessId = _businessId, CategoryId = _categoryId, ItemCode = "BAD", Name = "Bad reference" };
            object invalid = relation switch
            {
                "category-type" => new CategoryType { BusinessId = _businessId, CategoryId = foreignCategory.Id, Name = "Bad" },
                "catalog-category" => new CatalogItem { BusinessId = _businessId, CategoryId = foreignCategory.Id, ItemCode = "BAD", Name = "Bad" },
                "catalog-type" => AssignCatalog(catalog, type: foreignType.Id),
                "catalog-type-category-mismatch" => AssignCatalog(catalog, type: wrongType.Id),
                "catalog-last-supplier" => AssignCatalog(catalog, supplier: foreignSupplier.Id),
                "catalog-last-broker" => AssignCatalog(catalog, broker: foreignBroker.Id),
                "variant" => new CatalogVariant { BusinessId = _businessId, CatalogItemId = foreignCatalog.Id, Name = "Bad" },
                "supplier-item-supplier" => new SupplierItem { BusinessId = _businessId, SupplierId = foreignSupplier.Id, CatalogItemId = _catalogItemId },
                "supplier-item-catalog" => new SupplierItem { BusinessId = _businessId, SupplierId = _supplierId, CatalogItemId = foreignCatalog.Id },
                "broker-link-broker" => new BrokerSupplier { BusinessId = _businessId, BrokerId = foreignBroker.Id, SupplierId = _supplierId },
                "broker-link-supplier" => new BrokerSupplier { BusinessId = _businessId, BrokerId = ownBroker.Id, SupplierId = foreignSupplier.Id },
                "price-supplier" => new SupplierItemPrice { BusinessId = _businessId, SupplierId = foreignSupplier.Id, CatalogItemId = _catalogItemId },
                "price-catalog" => new SupplierItemPrice { BusinessId = _businessId, SupplierId = _supplierId, CatalogItemId = foreignCatalog.Id },
                "price-source-purchase" => new SupplierItemPrice { BusinessId = _businessId, SupplierId = _supplierId, CatalogItemId = _catalogItemId, SourcePurchaseId = foreignOrder.Id },
                "purchase-supplier" => new PurchaseOrder { BusinessId = _businessId, SupplierId = foreignSupplier.Id, OrderNumber = "BAD" },
                "purchase-broker" => new PurchaseOrder { BusinessId = _businessId, SupplierId = _supplierId, BrokerId = foreignBroker.Id, OrderNumber = "BAD" },
                "line-purchase" => new PurchaseItem { BusinessId = _businessId, PurchaseOrderId = foreignOrder.Id, CatalogItemId = _catalogItemId },
                "line-catalog" => new PurchaseItem { BusinessId = _businessId, PurchaseOrderId = ownOrder.Id, CatalogItemId = foreignCatalog.Id },
                "stock-catalog" => new StockMovement { BusinessId = _businessId, CatalogItemId = foreignCatalog.Id, CreatedById = _userId, MovementType = "Test" },
                _ => throw new InvalidOperationException("Unknown relationship test.")
            };
            _context.Add(invalid);
            var error = await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
            Assert.Equal("23503", Assert.IsType<Npgsql.PostgresException>(error.InnerException).SqlState);
            await transaction.RollbackAsync(); _context.ChangeTracker.Clear();
            Assert.Equal(50, (await _context.CatalogItems.SingleAsync(i => i.Id == _catalogItemId)).CurrentStock);
            Assert.Empty(await _context.StockMovements.ToListAsync());
        }

        private static CatalogItem AssignCatalog(CatalogItem item, Guid? type = null, Guid? supplier = null, Guid? broker = null)
        {
            item.TypeId = type; item.LastSupplierId = supplier; item.LastBrokerId = broker; return item;
        }

        [Fact]
        public async Task PostgreSqlPersistsBackendCalculatedLineDiscountAndTax()
        {
            var input = NewOrder(); input.Items[0].OrderedQuantity = 2; input.Items[0].UnitPrice = 100;
            input.Items[0].DiscountPercent = 10; input.Items[0].TaxPercent = 5;
            var service = CreatePurchaseService(); var preview = await service.PreviewAsync(input);
            (await _context.Purchases.CountAsync()).Should().Be(0);
            var created = await service.CreatePurchaseOrderAsync(input);
            _context.ChangeTracker.Clear(); var persisted = await service.GetPurchaseOrderByIdAsync(created.Id);
            persisted.Subtotal.Should().Be(180); persisted.TaxTotal.Should().Be(9); persisted.GrandTotal.Should().Be(189);
            persisted.GrandTotal.Should().Be(preview.GrandTotal); persisted.Items[0].DiscountPercent.Should().Be(10);
            persisted.Items[0].TaxPercent.Should().Be(5); persisted.Items[0].LineTotal.Should().Be(189);
            (await _context.StockMovements.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task PartialReceiptRetryWithStaleVersionCannotAddStockTwice()
        {
            var service = CreatePurchaseService();
            var draft = await service.CreatePurchaseOrderAsync(NewOrder());
            await service.UpdateStatusAsync(draft.Id, PurchaseStatus.Confirmed, draft.Version);
            var dispatched = await service.UpdateStatusAsync(draft.Id, PurchaseStatus.Dispatched);
            await service.UpdateStatusAsync(draft.Id, PurchaseStatus.Arrived);
            var verified = await service.UpdateStatusAsync(draft.Id, PurchaseStatus.Verified);
            var request = new ReceivePurchaseDto { ExpectedVersion = verified.Version,
                Items = [new() { PurchaseItemId = draft.Items[0].Id, ReceivedQuantityDelta = 2 }] };
            var first = await service.ReceiveItemsAsync(draft.Id, request);
            first.Version.Should().NotBe(verified.Version);
            first.Status.Should().Be(PurchaseStatus.Verified);
            var retry = () => service.ReceiveItemsAsync(draft.Id, request);
            await retry.Should().ThrowAsync<InvalidOperationException>().WithMessage("PURCHASE_VERSION_CONFLICT");
            _context.ChangeTracker.Clear();
            (await _context.CatalogItems.SingleAsync()).CurrentStock.Should().Be(52);
            (await _context.StockMovements.CountAsync()).Should().Be(1);
            (await _context.PurchaseItems.SingleAsync()).ReceivedQuantity.Should().Be(2);
        }

        [Fact]
        public async Task SeparateContextsRejectStalePurchaseUpdate()
        {
            var created = await CreatePurchaseService().CreatePurchaseOrderAsync(NewOrder());
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options;
            await using var other = new AppDbContext(options, _tenant);
            var stale = await other.Purchases.SingleAsync(p => p.Id == created.Id);
            await CreatePurchaseService().UpdateStatusAsync(created.Id, PurchaseStatus.Confirmed);
            stale.Notes = "stale overwrite";
            await FluentActions.Awaiting(() => other.SaveChangesAsync()).Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task InvalidReceiptBatchRollsBackAndReleasesTransaction()
        {
            var service = CreatePurchaseService();
            var created = await service.CreatePurchaseOrderAsync(NewOrder());
            await service.UpdateStatusAsync(created.Id, PurchaseStatus.Confirmed);
            await service.UpdateStatusAsync(created.Id, PurchaseStatus.Dispatched);
            await service.UpdateStatusAsync(created.Id, PurchaseStatus.Arrived);
            await service.UpdateStatusAsync(created.Id, PurchaseStatus.Verified);
            var invalid = () => service.ReceiveItemsAsync(created.Id, new() { Items = [
                new() { PurchaseItemId = created.Items[0].Id, ReceivedQuantityDelta = 1 },
                new() { PurchaseItemId = Guid.NewGuid(), ReceivedQuantityDelta = 1 }] });
            await invalid.Should().ThrowAsync<ArgumentException>();
            _context.Database.CurrentTransaction.Should().BeNull();
            (await _context.CatalogItems.SingleAsync()).CurrentStock.Should().Be(50);
            (await _context.StockMovements.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task PostgreSqlReportValuationUsesConfirmedCostAndIgnoresDrafts()
        {
            var service = CreatePurchaseService();
            var created = await service.CreatePurchaseOrderAsync(NewOrder());
            await service.UpdateStatusAsync(created.Id, PurchaseStatus.Confirmed);
            var draft = NewOrder(); draft.Items[0].UnitPrice = 999;
            await service.CreatePurchaseOrderAsync(draft);
            var report = new ReportService(_context);
            (await report.GetStockAnalyticsAsync(_businessId)).EstimatedInventoryValue.Should().Be(250);
            (await report.GetSpendAnalyticsAsync(_businessId, DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-1), DateTimeKind.Unspecified),
                DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Unspecified)))
                .Sum(x => x.TotalSpend).Should().Be(50);
        }

        [Fact]
        public async Task DatabaseRefusesCatalogDeletionWhenStockHistoryExists()
        {
            await new StockService(_context, _user).AdjustStockAsync(_catalogItemId, new() { QuantityDelta = 1,
                ExpectedVersion = (await _context.CatalogItems.SingleAsync()).RowVersion, Reason = "Audit test" });
            await FluentActions.Awaiting(() => _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM \"CatalogItems\" WHERE \"Id\" = {0}", _catalogItemId)).Should().ThrowAsync<Npgsql.PostgresException>();
            (await _context.StockMovements.CountAsync()).Should().Be(1);
            (await _context.CatalogItems.CountAsync()).Should().Be(1);
        }

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
                Items = new List<UpsertPurchaseItemDto>
                {
                    new UpsertPurchaseItemDto
                    {
                        CatalogItemId = _catalogItemId,
                        OrderedQuantity = 100m,
                        UnitPrice = 5.00m,
                        TaxPercent = 2m,
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
            await sut.UpdateStatusAsync(created.Id, PurchaseStatus.Arrived);
            var verified = await sut.UpdateStatusAsync(created.Id, PurchaseStatus.Verified);
            verified.VerifiedAt.Should().NotBeNull();
            verified.VerifiedById.Should().Be(_userId);

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
            movement.ReferenceType.Should().Be("PurchaseOrder");
            movement.ReferenceId.Should().Be(created.Id.ToString());
            movement.QuantityDelta.Should().Be(100m);
            movement.QuantityBefore.Should().Be(50m);
            movement.QuantityAfter.Should().Be(150m);
            (await sut.GetActivityAsync(created.Id)).Should().HaveCount(6);
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
