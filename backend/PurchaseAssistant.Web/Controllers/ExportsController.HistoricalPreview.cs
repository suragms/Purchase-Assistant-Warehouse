using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.Services;
using PurchaseAssistant.Infrastructure.Services;
using System.Text.Json;

namespace PurchaseAssistant.Web.Controllers;
public partial class ExportsController
{
    [HttpPost("historical/preview"), Authorize(Roles = "Owner,SuperAdmin"), Authorize(Policy = "RequireCatalogEdit"),
        Authorize(Policy = "RequirePurchaseEdit"), Authorize(Policy = "RequirePurchaseView"), RequestSizeLimit(1024)]
    public IActionResult HistoricalPreview([FromBody] JsonElement request)
    {
        Response.Headers.CacheControl = "no-store";
        // Strict selector-only boundary: no files, arbitrary rows, tenant IDs or caller provenance.
        if (request.ValueKind != JsonValueKind.Object || request.EnumerateObject().Count() != 1 ||
            !request.TryGetProperty("fixtureId", out var fixture) || fixture.ValueKind != JsonValueKind.String ||
            !HistoricalPreviewFixtures.Suites.Contains(fixture.GetString()) || user.UserId == null)
            return BadRequest(new { message = "Select a supported synthetic fixture; historical uploads are unavailable." });
        var data = HistoricalPreviewFixtures.Create(fixture.GetString()!, Business, user.UserId.Value);
        return Ok(HistoricalMetadataValidator.Preview(data.Input, data.Targets, user));
    }
}
