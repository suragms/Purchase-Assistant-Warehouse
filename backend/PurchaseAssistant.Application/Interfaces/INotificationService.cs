using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Notifications;
using PurchaseAssistant.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(Guid userId, int page, int pageSize, bool onlyUnread);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);

        // Internal application usage
        Task CreateNotificationAsync(Guid businessId, Guid userId, NotificationType type, string title, string message, string? referenceType = null, Guid? referenceId = null);
    }
}
