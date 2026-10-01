using PurchaseAssistant.Domain.Common;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Domain.Entities
{
    public class Notification : TenantEntity
    {
        public string? DedupeKey { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
    }
}
