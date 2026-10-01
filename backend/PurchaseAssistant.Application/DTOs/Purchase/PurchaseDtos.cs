using PurchaseAssistant.Application.DTOs;
using System;
using System.Collections.Generic;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Application.DTOs.Purchase
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public uint Version { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public Guid? BrokerId { get; set; }
        public string? BrokerName { get; set; }
        public PurchaseStatus Status { get; set; }
        public PaymentState PaymentState { get; set; }
        public DeliveryState DeliveryState { get; set; }
        public string? Notes { get; set; }
        [FinancialField]
        public decimal Subtotal { get; set; }
        [FinancialField]
        public decimal TaxTotal { get; set; }
        [FinancialField]
        public decimal GrandTotal { get; set; }
        [FinancialField]
        public decimal PaidAmount { get; set; }
        [FinancialField]
        public decimal RemainingAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public int? PaymentDays { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? DispatchedAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedById { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid CatalogItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string CatalogItemName { get; set; } = string.Empty;
        [OperationalNumeric]
        public decimal OrderedQuantity { get; set; }
        [OperationalNumeric]
        public decimal ReceivedQuantity { get; set; }
        [FinancialField]
        public decimal UnitPrice { get; set; }
        [FinancialField]
        public decimal DiscountPercent { get; set; }
        [FinancialField]
        public decimal TaxPercent { get; set; }
        [OperationalNumeric]
        public decimal? KgPerUnit { get; set; }
        [FinancialField]
        public decimal? LandingCostPerKg { get; set; }
        [FinancialField]
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
    }

    public class UpsertPurchaseOrderDto
    {
        public string? PreviewToken { get; set; }
        public uint? ExpectedVersion { get; set; }
        public string? OrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? BrokerId { get; set; }
        public int? PaymentDays { get; set; }
        public string? Notes { get; set; }
        public List<UpsertPurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchasePreviewDto
    {
        [FinancialField]
        public decimal Subtotal { get; set; }
        [FinancialField]
        public decimal TaxTotal { get; set; }
        [FinancialField]
        public decimal GrandTotal { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
        public string PreviewToken { get; set; } = string.Empty;
    }

    public class PurchaseActivityDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DetailsJson { get; set; }
    }

    public class UpsertPurchaseItemDto
    {
        public Guid CatalogItemId { get; set; }
        [OperationalNumeric]
        public decimal OrderedQuantity { get; set; }
        [FinancialField]
        public decimal UnitPrice { get; set; }
        [FinancialField]
        public decimal DiscountPercent { get; set; }
        [FinancialField]
        public decimal TaxPercent { get; set; }
        [OperationalNumeric]
        public decimal? KgPerUnit { get; set; }
        [FinancialField]
        public decimal? LandingCostPerKg { get; set; }
        public string? Notes { get; set; }
    }

    public class ReceivePurchaseDto
    {
        public uint? ExpectedVersion { get; set; }
        public List<ReceivePurchaseItemDto> Items { get; set; } = new();
    }

    public class UpdatePurchasePaymentDto
    {
        public uint? ExpectedVersion { get; set; }
        [FinancialField]
        public decimal PaidAmount { get; set; }
    }

    public class ReceivePurchaseItemDto
    {
        public Guid PurchaseItemId { get; set; }
        [OperationalNumeric]
        public decimal ReceivedQuantityDelta { get; set; }
        public string? Notes { get; set; }
    }
}
