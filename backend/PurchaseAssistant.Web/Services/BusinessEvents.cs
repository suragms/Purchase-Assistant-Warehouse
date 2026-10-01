using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Infrastructure.Data;
using System.Collections.Concurrent;
using System.Security.Claims;
namespace PurchaseAssistant.Web.Services;
[Authorize(Policy = "RequireSelectedBusiness")]
public class BusinessEventsHub(BusinessEvents events) : Hub
{
    public override Task OnConnectedAsync() { events.Connect(Context.ConnectionId, Context.User!); return base.OnConnectedAsync(); }
    public override Task OnDisconnectedAsync(Exception? exception) { events.Disconnect(Context.ConnectionId); return base.OnDisconnectedAsync(exception); }
}
public class BusinessEvents(IHubContext<BusinessEventsHub> hub, IServiceScopeFactory scopes, ILogger<BusinessEvents> logger)
{
    private readonly ConcurrentDictionary<string, ClaimsPrincipal> connections = new();
    public void Connect(string id, ClaimsPrincipal principal) => connections[id] = principal.Clone();
    public void Disconnect(string id) => connections.TryRemove(id, out _);
    public static bool CanReceive(ClaimsPrincipal principal, Guid businessId, string eventType) =>
        principal.FindFirstValue("businessId") == businessId.ToString() &&
        (principal.FindFirstValue("role") is "Owner" or "SuperAdmin" || eventType == "notification.changed" ||
        principal.HasClaim("permissions", eventType.StartsWith("stock.") ? "stock.view" : "purchase.view"));
    public async Task Publish(Guid businessId, string eventType)
    {
        if (eventType is not ("stock.changed" or "stock.physical_counted" or "purchase.changed" or "notification.changed")) return;
        var message = new { id = Guid.NewGuid(), type = eventType, businessId, createdAt = DateTime.UtcNow };
        foreach (var (connectionId, stored) in connections.ToArray())
        {
            if (stored.FindFirstValue("businessId") != businessId.ToString()) continue;
            try {
                using var scope = scopes.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var principal = stored.Clone();
                if (!await CurrentUserService.ValidateSessionAsync(principal, db)) { Disconnect(connectionId); continue; }
                if (CanReceive(principal, businessId, eventType)) await hub.Clients.Client(connectionId).SendAsync("businessEvent", message);
            } catch (Exception ex) { logger.LogWarning(ex, "Realtime invalidation delivery failed"); }
        }
    }
}
// Actions publish only after a successful committed service operation. No business values enter events.
public class BusinessEventFilter(BusinessEvents events) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = await next();
        if (result.Exception != null || !new[] { "POST", "PUT", "PATCH", "DELETE" }.Contains(context.HttpContext.Request.Method)) return;
        var status = result.Result switch { ObjectResult o => o.StatusCode ?? 200, StatusCodeResult s => s.StatusCode, _ => context.HttpContext.Response.StatusCode };
        if (status >= 400 || !Guid.TryParse(context.HttpContext.User.FindFirstValue("businessId"), out var business)) return;
        var controller = context.Controller.GetType().Name; var path = context.HttpContext.Request.Path.Value ?? "";
        if (controller == "StockController") { if (path.Contains("physical")) await events.Publish(business, "stock.physical_counted"); await events.Publish(business, "stock.changed"); }
        if (controller == "PurchaseController" && !path.EndsWith("preview")) { await events.Publish(business, "purchase.changed"); if (path.Contains("receive")) await events.Publish(business, "stock.changed"); }
        if (controller == "ApiOperationController" && path.EndsWith("usage")) await events.Publish(business, "stock.changed");
        if (controller is "NotificationsController" or "DamageReportController") await events.Publish(business, "notification.changed");
    }
}
