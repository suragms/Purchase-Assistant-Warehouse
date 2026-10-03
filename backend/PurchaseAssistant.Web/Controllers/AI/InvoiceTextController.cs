using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PurchaseAssistant.Web.Services;
namespace PurchaseAssistant.Web.Controllers.AI;
[ApiController, Route("api/v1/ai/invoice-text"), Authorize(Policy = "RequirePurchaseCreate"), EnableRateLimiting("ai")]
public class InvoiceTextController(InvoiceTextService service) : ControllerBase
{
    public record InvoiceRequest(string Text, bool UseAi = false);
    [HttpPost, RequestSizeLimit(100000)] public async Task<IActionResult> Preview(InvoiceRequest request, CancellationToken ct) { Response.Headers.CacheControl = "private, no-store"; return Ok(await service.Preview(request.Text, request.UseAi, ct)); }
}
