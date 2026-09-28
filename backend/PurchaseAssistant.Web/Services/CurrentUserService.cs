using Microsoft.AspNetCore.Http;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PurchaseAssistant.Web.Services
{
    public class CurrentUserService : ICurrentUserService, ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(val, out var id) ? id : null;
            }
        }

        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public Guid? BusinessId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("businessId");
                return Guid.TryParse(val, out var id) ? id : null;
            }
        }

        public string Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue("role") ?? string.Empty;

        public IEnumerable<string> Permissions => _httpContextAccessor.HttpContext?.User?.FindAll("permissions").Select(c => c.Value) ?? Enumerable.Empty<string>();

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public Guid GetBusinessId() => BusinessId ?? Guid.Empty;
    }
}