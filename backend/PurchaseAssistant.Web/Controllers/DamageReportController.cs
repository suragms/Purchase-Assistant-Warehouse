using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/purchases/{purchaseOrderId}/damage-reports")]
    public class DamageReportController : ControllerBase
    {
        private readonly IPurchaseDamageService _damageService;

        public DamageReportController(IPurchaseDamageService damageService)
        {
            _damageService = damageService;
        }

        [HttpGet]
        [Authorize(Policy = "RequireDamageReport")]
        public async Task<ActionResult<List<DamageReportDto>>> GetDamageReports(Guid purchaseOrderId)
        {
            var reports = await _damageService.GetDamageReportsAsync(purchaseOrderId);
            return Ok(reports);
        }

        [HttpPost]
        [Authorize(Policy = "RequireDamageReport")]
        public async Task<ActionResult<DamageReportDto>> CreateDamageReport(
            Guid purchaseOrderId,
            [FromBody] CreateDamageReportDto dto)
        {
            try
            {
                var report = await _damageService.CreateDamageReportAsync(purchaseOrderId, dto);
                return CreatedAtAction(nameof(GetDamageReports), new { purchaseOrderId }, report);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{reportId}")]
        [Authorize(Policy = "RequireDamageApprove")]
        public async Task<ActionResult<DamageReportDto>> UpdateDamageReportStatus(
            [FromRoute] Guid purchaseOrderId,
            [FromRoute] Guid reportId,
            [FromBody] UpdateDamageReportStatusDto dto)
        {
            try
            {
                var report = await _damageService.UpdateDamageReportStatusAsync(purchaseOrderId, reportId, dto);
                return Ok(report);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("~/api/v1/damage-reports/pending-count")]
        [Authorize(Policy = "RequireDamageReport")]
        public async Task<ActionResult<PendingDamageReportsCountDto>> GetPendingCount()
        {
            var count = await _damageService.GetPendingCountAsync();
            return Ok(count);
        }
    }
}