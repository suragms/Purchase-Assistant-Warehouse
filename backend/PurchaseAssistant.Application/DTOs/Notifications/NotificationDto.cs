using PurchaseAssistant.Domain.Enums;
using System;

namespace PurchaseAssistant.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
    }
}
