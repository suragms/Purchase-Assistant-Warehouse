using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Stock;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Application.Interfaces;
using Xunit;

namespace PurchaseAssistant.UnitTests.Services
{
    public class StubTenantProvider : ITenantProvider
    {
        public Guid BusinessId { get; set; }
        public Guid GetBusinessId() => BusinessId;
    }

    public class StubCurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; set; }
        public string Email => "test@test.com";
        public Guid? BusinessId { get; set; }
        public string Role => "Admin";
        public System.Collections.Generic.IEnumerable<string> Permissions => new[] { "stock.manage" };
        public bool HasPermission(string permission) => true;
    }

    public class StockServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly StubCurrentUserService _currentUser;
        private readonly StockService _sut;
        private readonly Guid _businessId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public StockServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var stubTenant = new StubTenantProvider { BusinessId = _businessId };

            _context = new AppDbContext(options, stubTenant);

            _context.Businesses.Add(new Business { Id = _businessId, Name = "Test Business" });
            _context.Users.Add(new User { Id = _userId, Name = "Test", Email = "test@test.com", PasswordHash = "hash" });
            _context.Categories.Add(new Category { Id = Guid.NewGuid(), BusinessId = _businessId, Name = "Test Cat" });
            _context.SaveChanges();

            _currentUser = new StubCurrentUserService { BusinessId = _businessId, UserId = _userId };

            _sut = new StockService(_context, _currentUser);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task<CatalogItem> CreateTestItem(decimal current = 50, decimal reserved = 10, decimal physical = 45)
        {
            var cat = await _context.Categories.FirstAsync();
            var item = new CatalogItem
            {
                Id = Guid.NewGuid(),
                BusinessId = _businessId,
                CategoryId = cat.Id,
                Name = "Test Item",
                ItemCode = "TEST-01",
                CurrentStock = current,
                ReservedStock = reserved,
                PhysicalStock = physical,
                RowVersion = Guid.NewGuid()
            };

            _context.CatalogItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [Fact]
        public async Task GetStockItemsAsync_ShouldCalculateAvailableStockCorrectly()
        {
            // Arrange
            await CreateTestItem(current: 100, reserved: 20); // available: 80

            // Act
            var result = await _sut.GetStockItemsAsync(1, 10, null, null, null);

            // Assert
            result.Data.Should().HaveCount(1);
            result.Data[0].SystemStock.Should().Be(100);
            result.Data[0].ReservedStock.Should().Be(20);
            result.Data[0].AvailableStock.Should().Be(80);
        }

        [Fact]
        public async Task AdjustStockAsync_PositiveDelta_UpdatesSystemStockAndLogsMovement()
        {
            // Arrange
            var item = await CreateTestItem(current: 50, reserved: 0);
            var originalVersion = item.RowVersion;
            var req = new AdjustStockRequestDto { Reason = "Regression stock adjustment", QuantityDelta = 25, ExpectedVersion = originalVersion };

            // Act
            var res = await _sut.AdjustStockAsync(item.Id, req);

            // Assert
            res.SystemStock.Should().Be(75);
            var updatedItem = await _context.CatalogItems.FindAsync(item.Id);
            updatedItem!.RowVersion.Should().NotBe(originalVersion);

            var movement = await _context.StockMovements.SingleAsync();
            movement.QuantityDelta.Should().Be(25);
            movement.QuantityBefore.Should().Be(50);
            movement.QuantityAfter.Should().Be(75);
            movement.MovementType.Should().Be("AdjustmentIncrease");
        }

        [Fact]
        public async Task AdjustStockAsync_NegativeDeltaTooLarge_ThrowsConflictException()
        {
            // Arrange
            var item = await CreateTestItem(current: 50, reserved: 20); // Available is 30
            var req = new AdjustStockRequestDto { Reason = "Regression stock adjustment", QuantityDelta = -40, ExpectedVersion = item.RowVersion }; // Reduces by 40, leading to -10 available

            // Act & Assert
            var act = async () => await _sut.AdjustStockAsync(item.Id, req);
            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.WithMessage("INSUFFICIENT_STOCK");
        }

        [Fact]
        public async Task ReconcileStockAsync_NoVariance_ThrowsInvalidOperation()
        {
            // Arrange
            var item = await CreateTestItem(current: 50, physical: 50, reserved: 0);
            var req = new ReconcileStockRequestDto { ExpectedVersion = item.RowVersion };

            // Act & Assert
            var act = async () => await _sut.ReconcileStockAsync(item.Id, req);
            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.WithMessage("RECONCILE_NO_VARIANCE");
        }

        [Fact]
        public async Task ReconcileStockAsync_WithVariance_UpdatesSystemToPhysicalAndLogsDelta()
        {
            // Arrange
            var item = await CreateTestItem(current: 50, physical: 30, reserved: 0);
            var req = new ReconcileStockRequestDto { ExpectedVersion = item.RowVersion, Reason = "Audited" };

            // Act
            var res = await _sut.ReconcileStockAsync(item.Id, req);

            // Assert
            res.SystemStock.Should().Be(30);
            var movement = await _context.StockMovements.SingleAsync();
            movement.MovementType.Should().Be("Reconciliation");
            movement.QuantityDelta.Should().Be(-20); // physical - system
        }

        [Fact]
        public async Task WrongRowVersion_ThrowsConcurrencyException()
        {
            // Arrange
            var item = await CreateTestItem(current: 50);
            var req = new AdjustStockRequestDto { Reason = "Regression stock adjustment", QuantityDelta = 10, ExpectedVersion = Guid.NewGuid() }; // Invalid version

            // Act & Assert
            var act = async () => await _sut.AdjustStockAsync(item.Id, req);
            var ex = await act.Should().ThrowAsync<InvalidOperationException>();
            ex.WithMessage("STOCK_VERSION_CONFLICT");
        }
    }
}
