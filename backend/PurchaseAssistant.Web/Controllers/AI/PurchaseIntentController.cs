using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers.AI;

[ApiController]
[Route("api/v1/ai/purchase-intent")]
public class PurchaseIntentController : ControllerBase
{
    private readonly IPurchaseParsingService _parsingService;

    public PurchaseIntentController(IPurchaseParsingService parsingService)
    {
        _parsingService = parsingService;
    }

    [HttpPost("parse")]
    [Authorize(Policy = "RequirePurchaseCreate")]
    public async Task<ActionResult<PurchaseIntentCandidateDto>> ParsePurchaseIntent([FromBody] ParseRequestDto dto)
    {
        var result = await _parsingService.ParseAsync(dto.Prompt);
        return Ok(result);
    }
}

public class ParseRequestDto
{
    public string Prompt { get; set; } = string.Empty;
}
