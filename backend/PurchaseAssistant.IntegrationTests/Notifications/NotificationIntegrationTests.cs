using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using Xunit;

namespace PurchaseAssistant.IntegrationTests.Notifications
{
    public class NotificationIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Host=localhost;Database=warehouse_erp_dev;Username=modelbridge;Password=modelbridge";

        private AppDbContext _context = null!;
        private INotificationService _service = null!;
        private Guid _businessId;
        private Guid _userId;
        private Guid _otherUserId;
        private Guid _otherBusinessId;
        private class StubTenant : ITenantProvider { public Guid BId; public Guid GetBusinessId() => BId; }
        private StubTenant _tenant = null!;

        public async Task InitializeAsync()
        {
            _businessId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _otherUserId = Guid.NewGuid();
            _otherBusinessId = Guid.NewGuid();

            _tenant = new StubTenant { BId = _businessId };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
                .Options;

            _context = new AppDbContext(options, _tenant);
            _service = new NotificationService(_context);

            // DB init
            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Businesses\" (\"Id\", \"Name\", \"IsActive\", \"CreatedAt\") VALUES ({0}, 'Main', true, NOW()), ({1}, 'Other', true, NOW())",
                _businessId, _otherBusinessId);

            await _context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"Users\" (\"Id\", \"Name\", \"Email\", \"PasswordHash\", \"Status\", \"CreatedAt\") VALUES ({0}, 'U1', 'u1@test.com', 'h', 1, NOW()), ({1}, 'U2', 'u2@test.com', 'h', 1, NOW())",
                _userId, _otherUserId);
        }

        public async Task DisposeAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Notifications\" WHERE \"BusinessId\" IN ({0}, {1})", _businessId, _otherBusinessId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\" WHERE \"Id\" IN ({0}, {1})", _userId, _otherUserId);
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Businesses\" WHERE \"Id\" IN ({0}, {1})", _businessId, _otherBusinessId);
        }

        [Fact]
        public async Task CreateNotification_ShouldAddUnreadNotification()
        {
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.LowStock, "Low Stock Test", "Item X is low");

            var count = await _service.GetUnreadCountAsync(_userId);
            count.Should().Be(1);
        }

        [Fact]
        public async Task CreateNotification_DuplicateTypeAndReference_ShouldDeduplicate()
        {
            var refId = Guid.NewGuid();
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.OutOfStock, "Out of Stock 1", "Item Y", "CatalogItem", refId);
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.OutOfStock, "Out of Stock 2", "Item Y again", "CatalogItem", refId);

            var count = await _service.GetUnreadCountAsync(_userId);
            count.Should().Be(1, "Second notification with same reference logic/unread state should be deduplicated");
        }

        [Fact]
        public async Task GetNotifications_ShouldEnforceUserIsolation()
        {
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.System, "U1 Notif", "Msg for U1");
            await _service.CreateNotificationAsync(_businessId, _otherUserId, NotificationType.System, "U2 Notif", "Msg for U2");

            var u1Notifs = await _service.GetNotificationsAsync(_userId, 1, 10, false);
            u1Notifs.Data.Should().HaveCount(1);
            u1Notifs.Data.First().UserId.Should().Be(_userId);
        }

        [Fact]
        public async Task MarkAsRead_ShouldUpdateStateAndReadAt()
        {
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.System, "To Read", "Msg");
            var res = await _service.GetNotificationsAsync(_userId, 1, 10, false);
            var nId = res.Data.First().Id;

            await _service.MarkAsReadAsync(nId, _userId);

            var after = await _service.GetNotificationsAsync(_userId, 1, 10, false);
            after.Data.First().IsRead.Should().BeTrue();
            after.Data.First().ReadAt.Should().NotBeNull();

            var count = await _service.GetUnreadCountAsync(_userId);
            count.Should().Be(0);
        }

        [Fact]
        public async Task MarkAllAsRead_ShouldUpdateAllUnread()
        {
            var ref1 = Guid.NewGuid();
            var ref2 = Guid.NewGuid();
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.System, "N1", "M1", "Ref", ref1);
            await _service.CreateNotificationAsync(_businessId, _userId, NotificationType.System, "N2", "M2", "Ref", ref2);

            var initialCount = await _service.GetUnreadCountAsync(_userId);
            initialCount.Should().Be(2);

            await _service.MarkAllAsReadAsync(_userId);

            var finalCount = await _service.GetUnreadCountAsync(_userId);
            finalCount.Should().Be(0);
        }
    }
}
