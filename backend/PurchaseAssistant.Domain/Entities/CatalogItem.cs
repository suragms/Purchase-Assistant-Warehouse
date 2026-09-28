using System;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class CatalogItem : TenantEntity
    {
        public string ItemCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Guid? TypeId { get; set; }
        public CategoryType? Type { get; set; }

        public string DefaultUnit { get; set; } = "PCS"; // PCS, KG, BOX
        public decimal? KgPerUnit { get; set; }
        public decimal ReorderLevel { get; set; } = 0;

        // System calculated current stock
        public decimal CurrentStock { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public Guid? LastSupplierId { get; set; }
        public Supplier? LastSupplier { get; set; }

        public Guid? LastBrokerId { get; set; }
        public Broker? LastBroker { get; set; }

        // Optimistic concurrency token
        public Guid RowVersion { get; set; } = Guid.NewGuid();
    }
}