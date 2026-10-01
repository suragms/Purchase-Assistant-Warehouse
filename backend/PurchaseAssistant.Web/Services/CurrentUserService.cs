using Microsoft.AspNetCore.Http;
using PurchaseAssistant.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Domain.Enums;
using System.Text.Json;

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

        public static async Task<bool> ValidateSessionAsync(ClaimsPrincipal principal, AppDbContext db)
        {
            if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                || !Guid.TryParse(principal.FindFirstValue("sessionId"), out var sessionId)
                || sessionId == Guid.Empty
                || !await db.RefreshTokens.AnyAsync(t => t.UserId == userId && t.FamilyId == sessionId
                    && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
                || !await db.Users.AnyAsync(u => u.Id == userId && u.Status == UserStatus.Active))
                return false;
            if (principal.Identity is not ClaimsIdentity identity) return false;
            var businessClaim = principal.FindFirstValue("businessId");
            var membership = Guid.TryParse(businessClaim, out var businessId)
                ? await db.Memberships.AsNoTracking().FirstOrDefaultAsync(m => m.UserId == userId && m.BusinessId == businessId && m.Business.IsActive)
                : null;
            if (businessClaim != null && membership == null) return false;
            // Use current server membership permissions; revoked privileges take effect immediately.
            foreach (var claim in identity.FindAll("permissions").Concat(identity.FindAll("role")).Concat(identity.FindAll(ClaimTypes.Role)).ToList())
                identity.RemoveClaim(claim);
            if (membership != null)
            {
                List<string> permissions;
                try { permissions = JsonSerializer.Deserialize<List<string>>(membership.PermissionsJson ?? "[]") ?? new(); }
                catch (JsonException) { return false; }
                if (permissions.Any(string.IsNullOrWhiteSpace)) return false;
                identity.AddClaim(new Claim("role", membership.Role.ToString()));
                foreach (var permission in permissions) identity.AddClaim(new Claim("permissions", permission));
            }
            return true;
        }
    }
}
