using System.Collections.Generic;
using PurchaseAssistant.Domain.Common;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.Active;

        public string? Avatar { get; set; }
        public string? Timezone { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}