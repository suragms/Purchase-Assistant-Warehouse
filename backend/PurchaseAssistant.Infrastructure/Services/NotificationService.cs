using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Notifications;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(Guid userId, int page, int pageSize, bool onlyUnread)
        {
            page = Math.Clamp(page, 1, 10000);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (onlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt,
                    ReferenceType = n.ReferenceType,
                    ReferenceId = n.ReferenceId
                })
                .ToListAsync();

            return new PaginatedResult<NotificationDto>
            {
                Data = items,
                Meta = new PaginationMeta
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            };
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var n in unreadNotifications)
                {
                    n.IsRead = true;
                    n.ReadAt = now;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateNotificationAsync(Guid businessId, Guid userId, NotificationType type, string title, string message, string? referenceType = null, Guid? referenceId = null)
        {
            // Deduplication: prevent creating another identical Unread notification
            bool exists = await _context.Notifications
                .AnyAsync(n => n.BusinessId == businessId
                            && n.UserId == userId
                            && n.Type == type
                            && n.ReferenceId == referenceId
                            && !n.IsRead);
            if (exists)
                return;

            var notification = new Notification
            {
                BusinessId = businessId,
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
