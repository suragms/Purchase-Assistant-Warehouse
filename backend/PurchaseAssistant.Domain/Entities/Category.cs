using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class Category : TenantEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryType : TenantEntity
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }
}