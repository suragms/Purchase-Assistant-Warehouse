using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Reports;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<PurchaseCsvLine>> GetCsvPurchaseLinesAsync(Guid businessId, DateTime? start, DateTime? end, Guid? supplierId, CancellationToken ct);
        Task<(List<SupplierCsvRow> Suppliers, List<ItemCsvRow> Items)> GetCsvPackReportsAsync(Guid businessId, DateTime? start, DateTime? end, CancellationToken ct);
        Task<List<SpendAnalyticsDto>> GetSpendAnalyticsAsync(Guid businessId, DateTime startDate, DateTime endDate, string groupBy = "day");
        Task<PurchaseSummaryReportDto> GetPurchaseSummaryAsync(Guid businessId, DateTime startDate, DateTime endDate);
        Task<StockAnalyticsDto> GetStockAnalyticsAsync(Guid businessId);
        Task<PeriodComparisonDto> GetPeriodComparisonAsync(Guid businessId, DateTime startDate, DateTime endDate);
    }
}
