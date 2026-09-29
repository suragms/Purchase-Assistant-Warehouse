using System;
using System.Collections.Generic;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Application.DTOs.Purchase
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public Guid? BrokerId { get; set; }
        public string? BrokerName { get; set; }
        public PurchaseStatus Status { get; set; }
        public PaymentState PaymentState { get; set; }
        public DeliveryState DeliveryState { get; set; }
        public string? Notes { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? DispatchedAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
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
        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
    }

    public class UpsertPurchaseOrderDto
    {
        public string? OrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public Guid? BrokerId { get; set; }
        public string? Notes { get; set; }
        public decimal TaxTotal { get; set; }
        public List<UpsertPurchaseItemDto> Items { get; set; } = new();
    }

    public class UpsertPurchaseItemDto
    {
        public Guid CatalogItemId { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class ReceivePurchaseDto
    {
        public List<ReceivePurchaseItemDto> Items { get; set; } = new();
    }

    public class ReceivePurchaseItemDto
    {
        public Guid PurchaseItemId { get; set; }
        public decimal ReceivedQuantityDelta { get; set; }
        public string? Notes { get; set; }
    }
}
