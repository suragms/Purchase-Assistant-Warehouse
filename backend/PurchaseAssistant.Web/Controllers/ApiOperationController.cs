using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Operations;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/operations")]
    [Authorize(Policy = "RequireSelectedBusiness")]
    public class ApiOperationController : ControllerBase
    {
        private readonly IOperationsService _operationsService;

        public ApiOperationController(IOperationsService operationsService)
        {
            _operationsService = operationsService;
        }

        [HttpGet("checklist/today")]
        public async Task<ActionResult<ChecklistTodayDto>> GetTodayChecklist()
            => Ok(await _operationsService.GetTodayChecklistAsync());

        [HttpPost("checklist/{slot}/{taskKey}")]
        public async Task<IActionResult> CompleteTask(ChecklistSlot slot, string taskKey, [FromBody] ChecklistCompleteDto dto)
        {
            await _operationsService.CompleteChecklistTaskAsync(slot, taskKey, dto);
            return NoContent();
        }

        [HttpGet("usage/today")]
        public async Task<ActionResult<UsageTodayDto>> GetTodayUsage()
            => Ok(await _operationsService.GetTodayUsageAsync());

        [HttpPost("usage")]
        [Authorize(Policy = "RequireStockAdjust")]
        public async Task<ActionResult<UsageSummaryDto>> SubmitUsage([FromBody] UsageSubmitDto dto)
            => Ok(await _operationsService.SubmitUsageAsync(dto));

        [HttpPost("snapshot/materialize")]
        [Authorize(Policy = "RequireStockAdjust")]
        public async Task<IActionResult> Materialize()
        {
            await _operationsService.MaterializeSnapshotsAsync();
            return NoContent();
        }

        [HttpGet("checklist/templates")]
        public async Task<IActionResult> Templates() => Ok(await _operationsService.GetChecklistTemplatesAsync());
        [HttpPut("checklist/templates")]
        [Authorize(Roles = "Owner,Admin,Manager,SuperAdmin")]
        public async Task<IActionResult> Templates(List<ChecklistTemplateDto> templates) => Ok(await _operationsService.UpdateChecklistTemplatesAsync(templates));
        [HttpGet("checklist/summary")]
        public async Task<IActionResult> Summary(DateTime? startDate, DateTime? endDate) => Ok(await _operationsService.GetChecklistSummaryAsync(startDate, endDate));
        [HttpGet("reports/summary")]
        public async Task<IActionResult> Report() => Ok(await _operationsService.GetOperationsReportSummaryAsync());
    }
}
