using System;
using PurchaseAssistant.Domain.Common;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Domain.Entities
{
    public class Membership : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Role Role { get; set; }

        // JSONB column in PostgreSQL
        public string? PermissionsJson { get; set; }
    }
}