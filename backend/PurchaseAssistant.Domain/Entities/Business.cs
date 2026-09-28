using System.Collections.Generic;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class Business : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}