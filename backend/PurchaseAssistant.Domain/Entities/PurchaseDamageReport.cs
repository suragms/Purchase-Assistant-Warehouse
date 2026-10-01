using System;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    /// <summary>
    /// Records a damage / short-delivery / missing / returned item event
    /// for a confirmed purchase order. Verified reference: purchase_damage_report.py.
    /// </summary>
    public class PurchaseDamageReport : TenantEntity
    {
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        /// <summary>Optional FK to the catalog item; SET NULL on item delete.</summary>
        public Guid? CatalogItemId { get; set; }
        public CatalogItem? CatalogItem { get; set; }

        /// <summary>Denormalised item name captured at report time (≤500 chars).</summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>Positive quantity that was damaged/short/missing/returned.</summary>
        public decimal QtyDamaged { get; set; }

        /// <summary>Unit label captured at report time (≤32 chars).</summary>
        public string? Unit { get; set; }

        /// <summary>damaged | short | missing | returned</summary>
        public string DamageType { get; set; } = "damaged";

        /// <summary>torn_bag | wet_damage | wrong_item | short_weight | other</summary>
        public string? Reason { get; set; }

        /// <summary>pending | approved | returned | rejected</summary>
        public string Status { get; set; } = "pending";

        public string? PhotoUrl { get; set; }
        public string? Notes { get; set; }

        public Guid? ReportedByUserId { get; set; }
        public User? ReportedByUser { get; set; }
    }
}
