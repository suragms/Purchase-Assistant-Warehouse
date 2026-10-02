using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Web.Services;

namespace PurchaseAssistant.UnitTests;

public class RealtimeDeliveryTests
{
    private sealed class Fixture : IDisposable
    {
        public readonly Guid BusinessId = Guid.NewGuid();
        public readonly ServiceProvider Services;
        public readonly BusinessEvents Events;
        public readonly List<(string Client, object Message)> Delivered = [];
        public Fixture()
        {
            var database = Guid.NewGuid().ToString();
            Services = new ServiceCollection().AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(database)).BuildServiceProvider();
            var clients = new Mock<IHubClients>();
            clients.Setup(x => x.Client(It.IsAny<string>())).Returns((string id) => {
                var proxy = new Mock<ISingleClientProxy>();
                proxy.Setup(x => x.SendCoreAsync("businessEvent", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                    .Callback<string, object?[], CancellationToken>((_, arguments, _) => Delivered.Add((id, arguments.Single()!)))
                    .Returns(Task.CompletedTask);
                return proxy.Object;
            });
            var hub = new Mock<IHubContext<BusinessEventsHub>>(); hub.SetupGet(x => x.Clients).Returns(clients.Object);
            Events = new BusinessEvents(hub.Object, Services.GetRequiredService<IServiceScopeFactory>(), NullLogger<BusinessEvents>.Instance);
        }
        public async Task<(Guid User, Guid Session)> Connect(string connection, Guid business, string[] permissions, bool forgedOwner = false)
        {
            using var scope = Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!await db.Businesses.AnyAsync(x => x.Id == business)) db.Businesses.Add(new Business { Id = business, Name = "Synthetic realtime" });
            var user = new User { Id = Guid.NewGuid(), Name = connection, Email = connection + "@example.test", PasswordHash = "unused" };
            var session = Guid.NewGuid(); db.Users.Add(user);
            db.Memberships.Add(new Membership { BusinessId = business, UserId = user.Id, Role = Role.Staff, PermissionsJson = JsonSerializer.Serialize(permissions) });
            db.RefreshTokens.Add(new RefreshToken { UserId = user.Id, FamilyId = session, ExpiresAt = DateTime.UtcNow.AddHours(1), TokenHash = "unused" });
            await db.SaveChangesAsync();
            Events.Connect(connection, new ClaimsPrincipal(new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim("sessionId", session.ToString()),
                new Claim("businessId", business.ToString()), new Claim("role", forgedOwner ? "Owner" : "Staff"),
                new Claim("permissions", "purchase.view"), new Claim("permissions", "stock.view")], "synthetic")));
            return (user.Id, session);
        }
        public void Dispose() => Services.Dispose();
    }

    [Theory]
    [InlineData("purchase.changed", false)]
    [InlineData("notification.changed", true)]
    public async Task Committed_events_reach_each_current_authorized_member_once_and_never_another_tenant(string type, bool allMembers)
    {
        using var f = new Fixture(); await f.Connect("first", f.BusinessId, ["purchase.view"]); await f.Connect("second", f.BusinessId, ["purchase.view"]);
        await f.Connect("no-purchase-access", f.BusinessId, []); await f.Connect("foreign", Guid.NewGuid(), ["purchase.view"]);
        var purchase = Guid.NewGuid(); await f.Events.Publish(f.BusinessId, type, purchaseId: purchase);
        Assert.Equal(allMembers ? 3 : 2, f.Delivered.Count); Assert.DoesNotContain(f.Delivered, x => x.Client == "foreign");
        Assert.Equal(f.Delivered.Count, f.Delivered.Select(x => x.Client).Distinct().Count());
        var payloads = f.Delivered.Select(x => JsonSerializer.SerializeToElement(x.Message)).ToList();
        Assert.Single(payloads.Select(x => x.GetProperty("id").GetGuid()).Distinct());
        Assert.All(payloads, x => { Assert.Equal(f.BusinessId, x.GetProperty("businessId").GetGuid()); Assert.Equal(type, x.GetProperty("type").GetString()); Assert.Equal(purchase, x.GetProperty("payload").GetProperty("purchaseId").GetGuid()); Assert.False(x.TryGetProperty("grandTotal", out _)); });
    }
    [Theory]
    [InlineData("purchase.changed")]
    [InlineData("stock.changed")]
    public async Task Forged_and_revoked_permissions_are_rechecked_against_membership(string type)
    {
        using var f = new Fixture(); var actor = await f.Connect("revoked", f.BusinessId, ["purchase.view", "stock.view"], forgedOwner: true);
        using (var scope = f.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); (await db.Memberships.SingleAsync(x => x.UserId == actor.User)).PermissionsJson = "[]"; await db.SaveChangesAsync(); }
        await f.Events.Publish(f.BusinessId, type); Assert.Empty(f.Delivered);
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Revoked_or_expired_sessions_receive_no_notification(bool revoked)
    {
        using var f = new Fixture(); var actor = await f.Connect("inactive-session", f.BusinessId, []);
        using (var scope = f.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var token = await db.RefreshTokens.SingleAsync(x => x.FamilyId == actor.Session); if (revoked) token.RevokedAt = DateTime.UtcNow; else token.ExpiresAt = DateTime.UtcNow.AddMinutes(-1); await db.SaveChangesAsync(); }
        await f.Events.Publish(f.BusinessId, "notification.changed"); Assert.Empty(f.Delivered);
    }
    [Fact]
    public async Task Disconnect_removes_the_old_connection_before_reconnect()
    {
        using var f = new Fixture(); await f.Connect("old", f.BusinessId, ["purchase.view"]); f.Events.Disconnect("old"); await f.Connect("new", f.BusinessId, ["purchase.view"]);
        await f.Events.Publish(f.BusinessId, "purchase.changed"); Assert.Equal("new", Assert.Single(f.Delivered).Client);
    }
    [Fact]
    public async Task Unrecognized_event_families_are_not_published()
    {
        using var f = new Fixture(); await f.Connect("member", f.BusinessId, ["purchase.view"]); await f.Events.Publish(f.BusinessId, "invented.event"); Assert.Empty(f.Delivered);
    }
}
