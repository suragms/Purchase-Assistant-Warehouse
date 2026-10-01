using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [Authorize(Policy = "RequireSelectedBusiness")]
    [ApiController]
    [Route("api/v1/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public NotificationsController(INotificationService notificationService, ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool onlyUnread = false)
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            var result = await _notificationService.GetNotificationsAsync(userId.Value, page, pageSize, onlyUnread);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            var count = await _notificationService.GetUnreadCountAsync(userId.Value);
            return Ok(new { Count = count });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            await _notificationService.MarkAsReadAsync(id, userId.Value);
            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _currentUser.UserId;
            if (userId == null) return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId.Value);
            return NoContent();
        }
    }
}
