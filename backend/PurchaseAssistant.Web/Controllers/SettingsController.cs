using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs;
using PurchaseAssistant.Application.DTOs.Settings;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using System.Text.Json;
namespace PurchaseAssistant.Web.Controllers;
[ApiController, Route("api/v1/settings"), Authorize(Policy = "RequireSelectedBusiness")]
public class SettingsController(AppDbContext db, ICurrentUserService user, IBusinessLogoStorage logos) : ControllerBase
{
    private async Task<Business> Business() => await db.Businesses.SingleOrDefaultAsync(x => x.Id == user.BusinessId && x.IsActive) ?? throw new KeyNotFoundException();
    private BusinessProfileDto Map(Business b) => new() { Name = b.Name, BrandingTitle = b.BrandingTitle, BrandingLogoUrl = b.BrandingLogoUrl, GstNumber = b.GstNumber, Address = b.Address, Phone = b.Phone, ContactEmail = b.ContactEmail, Version = b.Version, HasUploadedLogo = b.LogoStorageKey != null, LogoUploadAvailable = logos.Available };
    [HttpGet("business")]
    public async Task<IActionResult> Profile() => Ok(Map(await Business()));
    [HttpPut("business"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> Profile(BusinessProfileDto dto)
    {
        var b = await Business(); if (b.Version != dto.Version) return Conflict(new { message = "Business profile changed. Reload before saving." });
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
        b.Name = dto.Name.Trim(); b.BrandingTitle = dto.BrandingTitle?.Trim(); b.BrandingLogoUrl = SafeImageUrl.Validate(dto.BrandingLogoUrl);
        b.GstNumber = dto.GstNumber?.Trim().ToUpperInvariant(); b.Address = dto.Address?.Trim(); b.Phone = dto.Phone?.Trim(); b.ContactEmail = dto.ContactEmail?.Trim().ToLowerInvariant();
        b.Version = Guid.NewGuid(); await db.SaveChangesAsync(); return Ok(Map(b));
    }
    [HttpGet("notifications")]
    public async Task<IActionResult> Notifications()
    {
        var s = await db.Set<UserSettings>().SingleOrDefaultAsync(x => x.BusinessId == user.BusinessId && x.UserId == user.UserId);
        return Ok(s == null ? new UserSettingsDto() : new UserSettingsDto { NotificationsEnabled = s.NotificationsEnabled, NotificationKinds = JsonSerializer.Deserialize<string[]>(s.NotificationKindsJson) ?? [] });
    }
    [HttpPut("notifications")]
    public async Task<IActionResult> Notifications(UserSettingsDto dto)
    {
        var allowed = new UserSettingsDto().NotificationKinds;
        if (dto.NotificationKinds == null || dto.NotificationKinds.Length > allowed.Length || dto.NotificationKinds.Any(x => !allowed.Contains(x)) || dto.NotificationKinds.Distinct().Count() != dto.NotificationKinds.Length) throw new ArgumentException("Invalid notification settings.");
        var s = await db.Set<UserSettings>().SingleOrDefaultAsync(x => x.BusinessId == user.BusinessId && x.UserId == user.UserId);
        if (s == null) { s = new UserSettings { BusinessId = user.BusinessId!.Value, UserId = user.UserId!.Value }; db.Add(s); }
        s.NotificationsEnabled = dto.NotificationsEnabled; s.NotificationKindsJson = JsonSerializer.Serialize(dto.NotificationKinds); await db.SaveChangesAsync(); return Ok(dto);
    }
    [HttpPost("business/logo"), Authorize(Roles = "Owner,SuperAdmin"), RequestSizeLimit(2200000)]
    public async Task<IActionResult> UploadLogo(IFormFile file, [FromForm] Guid version, CancellationToken ct)
    {
        if (!logos.Available) return StatusCode(503, new { message = "Logo storage is not configured on this host." });
        if (file.Length is 0 or > LogoImageValidator.MaxBytes) return BadRequest(new { message = "Use a JPEG, PNG or WebP image up to 2 MB." });
        var b = await Business(); if (b.Version != version) return Conflict();
        using var buffer = new MemoryStream(); await file.CopyToAsync(buffer, ct);
        var png = LogoImageValidator.ValidateAndNormalize(buffer.ToArray(), file.ContentType, file.FileName);
        var key = await logos.StoreAsync(b.Id, png, ct); b.LogoStorageKey = key; b.Version = Guid.NewGuid(); await db.SaveChangesAsync(ct);
        return Ok(Map(b));
    }
    [HttpGet("business/logo")]
    public async Task<IActionResult> Logo(CancellationToken ct)
    {
        var b = await Business(); if (b.LogoStorageKey == null || !logos.Available) return NotFound();
        var bytes = await logos.ReadAsync(b.Id, b.LogoStorageKey, ct); if (bytes == null) return NotFound();
        Response.Headers.CacheControl = "private, no-store"; Response.Headers.XContentTypeOptions = "nosniff";
        return File(bytes, "image/png");
    }
    [HttpDelete("business/logo"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> DeleteLogo([FromQuery] Guid version, CancellationToken ct)
    {
        var b = await Business(); if (b.Version != version) return Conflict();
        b.LogoStorageKey = null; b.BrandingLogoUrl = null; b.Version = Guid.NewGuid(); await db.SaveChangesAsync(ct);
        return Ok(Map(b));
    }
}
