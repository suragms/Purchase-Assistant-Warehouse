using System;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class PurchaseItem : TenantEntity
    {
        public string Unit { get; set; } = "PCS";
        public string FreightType { get; set; } = "separate";
        public decimal? FreightAmount { get; set; }
        public decimal? DeliveredCharge { get; set; }
        public decimal? BilltyCharge { get; set; }

        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;

        public decimal OrderedQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal? KgPerUnit { get; set; }
        public decimal? LandingCostPerKg { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }
    }
}
