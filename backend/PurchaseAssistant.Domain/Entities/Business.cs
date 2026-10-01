using System.Collections.Generic;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class Business : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? BrandingTitle { get; set; }
        public string? BrandingLogoUrl { get; set; }
        public string? LogoStorageKey { get; set; }
        public string? GstNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? ContactEmail { get; set; }
        public Guid Version { get; set; } = Guid.NewGuid();
        public bool IsActive { get; set; } = true;

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}