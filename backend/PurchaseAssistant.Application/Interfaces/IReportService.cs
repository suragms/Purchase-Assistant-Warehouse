using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Reports;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<SpendAnalyticsDto>> GetSpendAnalyticsAsync(Guid businessId, DateTime startDate, DateTime endDate, string groupBy = "day");
        Task<PurchaseSummaryReportDto> GetPurchaseSummaryAsync(Guid businessId, DateTime startDate, DateTime endDate);
        Task<StockAnalyticsDto> GetStockAnalyticsAsync(Guid businessId);
        Task<PeriodComparisonDto> GetPeriodComparisonAsync(Guid businessId, DateTime startDate, DateTime endDate);
    }
}
