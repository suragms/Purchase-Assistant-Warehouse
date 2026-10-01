using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class CatalogVariant : TenantEntity
    {
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Barcode { get; set; }
        public string? AttributesJson { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal? KgPerUnit { get; set; }
        public Guid RowVersion { get; set; } = Guid.NewGuid();
    }

    public class SupplierItem : TenantEntity
    {
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;
        public string? SupplierItemCode { get; set; }
        public bool IsDefault { get; set; } = false;
        public string? Notes { get; set; }
    }

    public class BrokerSupplier : TenantEntity
    {
        public Guid BrokerId { get; set; }
        public Broker Broker { get; set; } = null!;
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
    }

    public class SupplierItemPrice : TenantEntity
    {
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;
        public string Unit { get; set; } = "PCS";
        public decimal Price { get; set; }
        public decimal? PricePerKg { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public Guid? SourcePurchaseId { get; set; }
    }
}
