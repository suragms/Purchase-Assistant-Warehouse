using System;
using System.Collections.Generic;

namespace PurchaseAssistant.Application.DTOs.Reports
{
    public class SpendAnalyticsDto
    {
        public string PeriodLabel { get; set; } = string.Empty; // e.g. "2026-09-01" or "Week 38"
        public decimal TotalSpend { get; set; }
        public int PurchaseCount { get; set; }
    }

    public class SummaryItemDto
    {
        public string Key { get; set; } = string.Empty;
        public decimal TotalSpend { get; set; }
        public int Count { get; set; }
    }

    public class PurchaseSummaryReportDto
    {
        public List<SummaryItemDto> BySupplier { get; set; } = new();
        public List<SummaryItemDto> ByCategory { get; set; } = new();
        public List<SummaryItemDto> ByStatus { get; set; } = new();
    }

    public class StockAnalyticsDto
    {
        public int TotalCatalogItems { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal EstimatedInventoryValue { get; set; }
        public int TotalMovementsCount { get; set; }
    }

    public class PeriodComparisonDto
    {
        public DateTime CurrentStartDate { get; set; }
        public DateTime CurrentEndDate { get; set; }
        public DateTime PreviousStartDate { get; set; }
        public DateTime PreviousEndDate { get; set; }

        public decimal CurrentPeriodSpend { get; set; }
        public decimal PreviousPeriodSpend { get; set; }
        public decimal SpendChangePercentage { get; set; }

        public int CurrentPeriodOrders { get; set; }
        public int PreviousPeriodOrders { get; set; }
        public decimal OrdersChangePercentage { get; set; }

        public decimal CurrentPeriodAvgOrderValue { get; set; }
        public decimal PreviousPeriodAvgOrderValue { get; set; }
        public decimal AvgOrderValueChangePercentage { get; set; }
    }
}
