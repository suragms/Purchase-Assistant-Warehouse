using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Infrastructure.Services.AI;

namespace PurchaseAssistant.Web.Controllers.AI;

[ApiController, Route("api/v1/settings/ai"), Authorize(Policy = "RequireSelectedBusiness"), Authorize(Roles = "Owner,Admin,SuperAdmin")]
public class ProviderSettingsController(AiRuntimeSettings settings) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await settings.GetAsync(ct));
    [HttpPut] public async Task<IActionResult> Save(AiProviderPolicy policy, CancellationToken ct) => Ok(await settings.SaveAsync(policy, ct));
}
