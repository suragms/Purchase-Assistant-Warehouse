using System;
using System.Collections.Generic;
using PurchaseAssistant.Domain.Common;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Domain.Entities
{
    public class PurchaseOrder : TenantEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public Guid? BrokerId { get; set; }
        public Broker? Broker { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;
        public PaymentState PaymentState { get; set; } = PaymentState.Pending;
        public DeliveryState DeliveryState { get; set; } = DeliveryState.Pending;

        public string? Notes { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }

        public DateTime? ConfirmedAt { get; set; }
        public DateTime? DispatchedAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}
