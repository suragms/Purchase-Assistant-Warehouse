using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid FamilyId { get; set; } = Guid.NewGuid();
        public string TokenHash { get; set; } = string.Empty;
        public string? TokenDigest { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? UserAgent { get; set; }

        public bool IsActive => RevokedAt == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
