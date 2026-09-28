using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class Supplier : TenantEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Broker : TenantEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}