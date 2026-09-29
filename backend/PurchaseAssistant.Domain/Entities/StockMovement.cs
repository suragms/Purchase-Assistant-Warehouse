using System;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class StockMovement : TenantEntity
    {
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;

        public string MovementType { get; set; } = string.Empty; // e.g., AdjustmentIncrease, AdjustmentDecrease, PhysicalCount, Reconciliation

        public decimal QuantityDelta { get; set; }
        public decimal QuantityBefore { get; set; }
        public decimal QuantityAfter { get; set; }

        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
