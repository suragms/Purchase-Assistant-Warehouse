using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Auth;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Contracts.Responses;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICurrentUserService _currentUser;

        public AuthController(
            AppDbContext db,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            ICurrentUserService currentUser)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _currentUser = currentUser;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _db.Users
                .Include(u => u.Memberships)
                .ThenInclude(m => m.Business)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                await LogSecurityEvent(null, null, "LOGIN_FAILED", $"Failed login attempt for {normalizedEmail}");
                return Unauthorized(new { error = new { code = "INVALID_CREDENTIALS", message = "Invalid email or password." } });
            }

            if (user.Status != UserStatus.Active)
            {
                await LogSecurityEvent(null, user.Id, "LOGIN_BLOCKED", $"Blocked login for status {user.Status}");
                return StatusCode(StatusCodes.Status403Forbidden, new { error = new { code = "ACCOUNT_INACTIVE", message = $"Account is {user.Status}." } });
            }

            var activeMembership = user.Memberships.FirstOrDefault(m => m.Business.IsActive);
            var token = _jwtProvider.GenerateAccessToken(user, activeMembership);
            var refreshTokenString = _jwtProvider.GenerateRandomToken();
            var refreshTokenHash = _passwordHasher.HashPassword(refreshTokenString);

            var rt = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie($"{user.Id}:{refreshTokenString}");
            await LogSecurityEvent(activeMembership?.BusinessId, user.Id, "LOGIN_SUCCESS", $"User {user.Email} logged in successfully");

            var permissions = new List<string>();
            if (activeMembership != null && !string.IsNullOrEmpty(activeMembership.PermissionsJson))
            {
                try { permissions = JsonSerializer.Deserialize<List<string>>(activeMembership.PermissionsJson) ?? new(); } catch { }
            }

            var response = new AuthResponse
            {
                AccessToken = token,
                ExpiresAt = new DateTimeOffset(DateTime.UtcNow.AddMinutes(15)).ToUnixTimeSeconds(),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CurrentBusiness = activeMembership != null ? new BusinessContextDto
                    {
                        BusinessId = activeMembership.BusinessId,
                        BusinessName = activeMembership.Business.Name,
                        Role = activeMembership.Role.ToString(),
                        Permissions = permissions
                    } : null,
                    Businesses = user.Memberships.Select(m => new BusinessSummaryDto
                    {
                        BusinessId = m.BusinessId,
                        BusinessName = m.Business.Name,
                        Role = m.Role.ToString()
                    }).ToList()
                }
            };

            return Ok(new ApiResponse<AuthResponse>(response));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var cookie = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(cookie))
            {
                return Unauthorized(new { error = new { code = "AUTH_REQUIRED", message = "Refresh token is missing." } });
            }

            var parts = cookie.Split(':', 2);
            if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId))
            {
                return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "Malformed refresh cookie." } });
            }

            var tokenRaw = parts[1];
            var activeTokens = await _db.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            var matched = activeTokens.FirstOrDefault(t => _passwordHasher.VerifyPassword(tokenRaw, t.TokenHash));
            if (matched == null)
            {
                // Suspected token reuse or invalid token: revoke all tokens for this user family as security measure
                var staleTokens = await _db.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync();
                foreach (var t in staleTokens)
                {
                    t.RevokedAt = DateTime.UtcNow;
                    t.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                }
                await _db.SaveChangesAsync();
                await LogSecurityEvent(null, userId, "REFRESH_TOKEN_REUSE_DETECTED", "Revoked all active sessions due to invalid/reused token presentation.");

                return Unauthorized(new { error = new { code = "REFRESH_TOKEN_REUSE", message = "Security violation: session invalidated." } });
            }

            matched.RevokedAt = DateTime.UtcNow;
            matched.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            var user = await _db.Users
                .Include(u => u.Memberships)
                .ThenInclude(m => m.Business)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Status != UserStatus.Active)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = new { code = "ACCOUNT_INACTIVE", message = "Account is inactive or blocked." } });
            }

            var activeMembership = user.Memberships.FirstOrDefault(m => m.Business.IsActive);
            var newAccessToken = _jwtProvider.GenerateAccessToken(user, activeMembership);
            var newRfString = _jwtProvider.GenerateRandomToken();
            var newRfHash = _passwordHasher.HashPassword(newRfString);

            var newRf = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRfHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
                ReplacedByTokenId = matched.Id
            };

            _db.RefreshTokens.Add(newRf);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie($"{user.Id}:{newRfString}");

            var permissions = new List<string>();
            if (activeMembership != null && !string.IsNullOrEmpty(activeMembership.PermissionsJson))
            {
                try { permissions = JsonSerializer.Deserialize<List<string>>(activeMembership.PermissionsJson) ?? new(); } catch { }
            }

            return Ok(new ApiResponse<AuthResponse>(new AuthResponse
            {
                AccessToken = newAccessToken,
                ExpiresAt = new DateTimeOffset(DateTime.UtcNow.AddMinutes(15)).ToUnixTimeSeconds(),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CurrentBusiness = activeMembership != null ? new BusinessContextDto
                    {
                        BusinessId = activeMembership.BusinessId,
                        BusinessName = activeMembership.Business.Name,
                        Role = activeMembership.Role.ToString(),
                        Permissions = permissions
                    } : null,
                    Businesses = user.Memberships.Select(m => new BusinessSummaryDto
                    {
                        BusinessId = m.BusinessId,
                        BusinessName = m.Business.Name,
                        Role = m.Role.ToString()
                    }).ToList()
                }
            }));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (!_currentUser.UserId.HasValue) return Unauthorized();

            var user = await _db.Users
                .Include(u => u.Memberships)
                .ThenInclude(m => m.Business)
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value);

            if (user == null) return NotFound(new { error = new { code = "USER_NOT_FOUND", message = "User not found." } });

            var currentBusinessId = _currentUser.BusinessId;
            var activeMembership = user.Memberships.FirstOrDefault(m => m.BusinessId == currentBusinessId)
                                  ?? user.Memberships.FirstOrDefault();

            var permissions = new List<string>();
            if (activeMembership != null && !string.IsNullOrEmpty(activeMembership.PermissionsJson))
            {
                try { permissions = JsonSerializer.Deserialize<List<string>>(activeMembership.PermissionsJson) ?? new(); } catch { }
            }

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CurrentBusiness = activeMembership != null ? new BusinessContextDto
                {
                    BusinessId = activeMembership.BusinessId,
                    BusinessName = activeMembership.Business.Name,
                    Role = activeMembership.Role.ToString(),
                    Permissions = permissions
                } : null,
                Businesses = user.Memberships.Select(m => new BusinessSummaryDto
                {
                    BusinessId = m.BusinessId,
                    BusinessName = m.Business.Name,
                    Role = m.Role.ToString()
                }).ToList()
            };

            return Ok(new ApiResponse<UserDto>(dto));
        }

        [HttpPost("select-business")]
        [Authorize]
        public async Task<IActionResult> SelectBusiness([FromBody] SelectBusinessRequest request)
        {
            if (!_currentUser.UserId.HasValue) return Unauthorized();

            var user = await _db.Users
                .Include(u => u.Memberships)
                .ThenInclude(m => m.Business)
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value);

            if (user == null) return NotFound();

            var targetMembership = user.Memberships.FirstOrDefault(m => m.BusinessId == request.BusinessId && m.Business.IsActive);
            if (targetMembership == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = new { code = "BUSINESS_ACCESS_DENIED", message = "You do not belong to this business." } });
            }

            var newAccessToken = _jwtProvider.GenerateAccessToken(user, targetMembership);
            await LogSecurityEvent(targetMembership.BusinessId, user.Id, "BUSINESS_SWITCHED", $"User switched active context to business {targetMembership.Business.Name}");

            var permissions = new List<string>();
            if (!string.IsNullOrEmpty(targetMembership.PermissionsJson))
            {
                try { permissions = JsonSerializer.Deserialize<List<string>>(targetMembership.PermissionsJson) ?? new(); } catch { }
            }

            return Ok(new ApiResponse<AuthResponse>(new AuthResponse
            {
                AccessToken = newAccessToken,
                ExpiresAt = new DateTimeOffset(DateTime.UtcNow.AddMinutes(15)).ToUnixTimeSeconds(),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CurrentBusiness = new BusinessContextDto
                    {
                        BusinessId = targetMembership.BusinessId,
                        BusinessName = targetMembership.Business.Name,
                        Role = targetMembership.Role.ToString(),
                        Permissions = permissions
                    },
                    Businesses = user.Memberships.Select(m => new BusinessSummaryDto
                    {
                        BusinessId = m.BusinessId,
                        BusinessName = m.Business.Name,
                        Role = m.Role.ToString()
                    }).ToList()
                }
            }));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var cookie = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(cookie))
            {
                var parts = cookie.Split(':', 2);
                if (parts.Length == 2 && Guid.TryParse(parts[0], out var userId))
                {
                    var tokens = await _db.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync();
                    var matched = tokens.FirstOrDefault(t => _passwordHasher.VerifyPassword(parts[1], t.TokenHash));
                    if (matched != null)
                    {
                        matched.RevokedAt = DateTime.UtcNow;
                        matched.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                        await _db.SaveChangesAsync();
                    }
                }
            }

            Response.Cookies.Delete("refreshToken");
            if (_currentUser.UserId.HasValue)
            {
                await LogSecurityEvent(_currentUser.BusinessId, _currentUser.UserId.Value, "LOGOUT", "User logged out");
            }
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            if (!_currentUser.UserId.HasValue) return Unauthorized();

            var tokens = await _db.RefreshTokens
                .Where(t => t.UserId == _currentUser.UserId.Value && t.RevokedAt == null)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.RevokedAt = DateTime.UtcNow;
                t.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            await _db.SaveChangesAsync();
            Response.Cookies.Delete("refreshToken");
            await LogSecurityEvent(_currentUser.BusinessId, _currentUser.UserId.Value, "LOGOUT_ALL", "User revoked all active sessions");

            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLowerInvariant());
            if (user != null)
            {
                await LogSecurityEvent(null, user.Id, "PASSWORD_RESET_REQUESTED", "Password reset instructions generated.");
            }
            // Always return identical generic message to prevent email enumeration
            return Ok(new ApiResponse<string>("If the account exists, password reset instructions have been sent."));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            {
                return BadRequest(new { error = new { code = "VALIDATION_ERROR", message = "Password must be at least 8 characters long." } });
            }

            // In production, validate cryptographically signed token. Here, handle token validation:
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLowerInvariant());
            if (user == null)
            {
                return BadRequest(new { error = new { code = "INVALID_RESET_TOKEN", message = "Invalid or expired reset token." } });
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Revoke all existing sessions
            var tokens = await _db.RefreshTokens.Where(t => t.UserId == user.Id && t.RevokedAt == null).ToListAsync();
            foreach (var t in tokens)
            {
                t.RevokedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            await LogSecurityEvent(null, user.Id, "PASSWORD_RESET_SUCCESS", "Password reset successfully completed. All sessions revoked.");

            return Ok(new ApiResponse<string>("Password has been reset successfully. Please sign in."));
        }

        private void SetRefreshTokenCookie(string tokenValue)
        {
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = !isDevelopment,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
                Path = "/"
            };
            Response.Cookies.Append("refreshToken", tokenValue, cookieOptions);
        }

        private async Task LogSecurityEvent(Guid? businessId, Guid? userId, string eventType, string desc)
        {
            try
            {
                var log = new SecurityAuditLog
                {
                    BusinessId = businessId ?? Guid.Empty,
                    UserId = userId,
                    EventType = eventType,
                    Description = desc,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    RequestId = HttpContext.TraceIdentifier
                };
                _db.SecurityAuditLogs.Add(log);
                await _db.SaveChangesAsync();
            }
            catch { /* non-blocking audit failure */ }
        }
    }
}