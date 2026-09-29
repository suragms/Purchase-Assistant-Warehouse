using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Domain.Enums;
using System;
using System.Collections.Generic;

namespace PurchaseAssistant.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public PurchaseMetricsDto PurchaseMetrics { get; set; } = new();
        public StockMetricsDto StockMetrics { get; set; } = new();
        public DeliveryMetricsDto DeliveryMetrics { get; set; } = new();

        public List<DashboardAlertDto> OperationalAlerts { get; set; } = new();
        public List<PurchaseOrderDto> RecentPurchases { get; set; } = new();
        public List<RecentStockActivityDto> RecentStockActivity { get; set; } = new();
    }

    public class PurchaseMetricsDto
    {
        public int TodayPurchasesCount { get; set; }
        public int PendingPurchasesCount { get; set; }
        public int ActivePurchasesCount { get; set; }
        public int CompletedPurchasesCount { get; set; }
        public decimal TotalPurchaseSpend { get; set; }
    }

    public class StockMetricsDto
    {
        public int TotalCatalogItems { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int ItermsWithPhysicalVariance { get; set; }
    }

    public class DeliveryMetricsDto
    {
        public int DraftPurchases { get; set; }
        public int ConfirmedPurchases { get; set; }
        public int DispatchedPurchases { get; set; }
        public int ArrivedPurchases { get; set; }
        public int VerificationPending { get; set; }
    }

    public class DashboardAlertDto
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
    }

    public class RecentStockActivityDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string CatalogItemName { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public decimal QuantityDelta { get; set; }
        public decimal BeforeQuantity { get; set; }
        public decimal AfterQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
    }
}
