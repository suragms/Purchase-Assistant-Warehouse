using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ICurrentUserService _currentUser;

        public ReportsController(IReportService reportService, ICurrentUserService currentUser)
        {
            _reportService = reportService;
            _currentUser = currentUser;
        }

        private Guid GetBusinessId()
        {
            return _currentUser.BusinessId ?? Guid.Empty;
        }

        [HttpGet("spend")]
        [Authorize(Policy = "RequireReportsView")]
        public async Task<ActionResult<List<SpendAnalyticsDto>>> GetSpendAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string groupBy = "day")
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            var businessId = GetBusinessId();
            var result = await _reportService.GetSpendAnalyticsAsync(businessId, start, end, groupBy);
            return Ok(result);
        }

        [HttpGet("purchases-summary")]
        [Authorize(Policy = "RequireReportsView")]
        public async Task<ActionResult<PurchaseSummaryReportDto>> GetPurchaseSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            var businessId = GetBusinessId();
            var result = await _reportService.GetPurchaseSummaryAsync(businessId, start, end);
            return Ok(result);
        }

        [HttpGet("stock-analytics")]
        [Authorize(Policy = "RequireReportsView")]
        public async Task<ActionResult<StockAnalyticsDto>> GetStockAnalytics()
        {
            var businessId = GetBusinessId();
            var result = await _reportService.GetStockAnalyticsAsync(businessId);
            return Ok(result);
        }

        [HttpGet("comparison")]
        [Authorize(Policy = "RequireReportsView")]
        public async Task<ActionResult<PeriodComparisonDto>> GetPeriodComparison(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            var businessId = GetBusinessId();
            var result = await _reportService.GetPeriodComparisonAsync(businessId, start, end);
            return Ok(result);
        }
    }
}
