using PurchaseAssistant.Domain.Common;
using System.Text.Json;

namespace PurchaseAssistant.Domain.Entities
{
    public class SecurityAuditLog : TenantEntity
    {
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? RequestId { get; set; }
        public string? MetadataJson { get; set; }
    }
}