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
        [HttpGet("usage/summary")]
        public async Task<IActionResult> UsageSummary(DateTime? date) => Ok(await _operationsService.GetUsageSummaryAsync(date));
        [HttpGet("snapshots")]
        public async Task<IActionResult> Snapshots(DateTime? fromDate, DateTime? toDate, Guid? itemId) => Ok(await _operationsService.GetSnapshotsAsync(fromDate, toDate, itemId));
        [HttpGet("tasks")]
        public async Task<IActionResult> Tasks(string? status, Guid? staffId) => Ok(await _operationsService.GetStaffTasksAsync(status, staffId));
        [HttpGet("tasks/assignees"), Authorize(Roles = "Owner,Admin,SuperAdmin")]
        public async Task<IActionResult> Assignees() => Ok(await _operationsService.GetTaskAssigneesAsync());
        [HttpPost("tasks"), Authorize(Roles = "Owner,Admin,SuperAdmin")]
        public async Task<IActionResult> CreateTask(StaffTaskCreateDto dto) => Ok(await _operationsService.CreateStaffTaskAsync(dto));
        [HttpPost("tasks/{id:guid}/accept")]
        public async Task<IActionResult> AcceptTask(Guid id, StaffTaskActionDto dto) => Ok(await _operationsService.ActOnStaffTaskAsync(id, dto, true));
        [HttpPost("tasks/{id:guid}/complete")]
        public async Task<IActionResult> CompleteStaffTask(Guid id, StaffTaskActionDto dto) => Ok(await _operationsService.ActOnStaffTaskAsync(id, dto, false));
        [HttpGet("tasks/performance"), Authorize(Roles = "Owner,Admin,SuperAdmin")]
        public async Task<IActionResult> Performance() => Ok(await _operationsService.GetStaffPerformanceAsync());
        [HttpGet("owner-dashboard"), Authorize(Roles = "Owner,Admin,SuperAdmin")]
        public async Task<IActionResult> OwnerDashboard() => Ok(await _operationsService.GetOwnerDashboardAsync());
    }
}
