using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PurchaseAssistant.Web.Services;
namespace PurchaseAssistant.Web.Controllers;
[ApiController, Route("api/v1/purchases/{id:guid}/delivery/whatsapp"), Authorize(Policy = "RequirePurchaseView"), Authorize(Roles = "Owner,SuperAdmin"), EnableRateLimiting("ai")]
public class WhatsAppDeliveryController(WhatsAppDeliveryService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Preview(Guid id, CancellationToken ct) { Response.Headers.CacheControl = "private, no-store"; return Ok(await service.Preview(id, ct)); }
    [HttpPost] public async Task<IActionResult> Send(Guid id, DeliveryRequest request, CancellationToken ct) => Ok(await service.Send(id, request, ct));
}
