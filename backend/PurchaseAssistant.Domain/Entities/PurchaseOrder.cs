using System;
using System.Collections.Generic;
using PurchaseAssistant.Domain.Common;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Domain.Entities
{
    public class PurchaseOrder : TenantEntity
    {
        public decimal HeaderDiscountPercent { get; set; }
        public string FreightType { get; set; } = "separate";
        public decimal FreightAmount { get; set; }
        public decimal DeliveredCharge { get; set; }
        public decimal BilltyCharge { get; set; }
        public string CommissionMode { get; set; } = "percent";
        public decimal CommissionPercent { get; set; }
        public decimal CommissionAmount { get; set; }

        public string OrderNumber { get; set; } = string.Empty;
        // PostgreSQL's built-in xmin supplies a store-generated concurrency token.
        public uint Version { get; set; }
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
        public decimal PaidAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public int? PaymentDays { get; set; }

        public DateTime? ConfirmedAt { get; set; }
        public DateTime? DispatchedAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedById { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}
