using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Settings;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using System.Security.Cryptography;

namespace PurchaseAssistant.Web.Services;

public class ProviderCredentialService(AppDbContext db, ICurrentUserService user, IDataProtectionProvider protection) : IProviderCredentialResolver
{
    public static readonly string[] AllowedTypes = ["openrouter_key", "gemini_key", "groq_key", "openai_key", "whatsapp_api_key", "whatsapp_staff_number", "whatsapp_phone_number_id"];
    private Guid Business => user.BusinessId ?? throw new UnauthorizedAccessException();
    private void RequireOwner() { if (user.Role is not ("Owner" or "Admin" or "SuperAdmin")) throw new UnauthorizedAccessException(); }
    private IDataProtector Protector(string type) => protection.CreateProtector("ProviderCredentials", Business.ToString(), type);
    private static CredentialStatusDto Status(ProviderCredential row) => new(row.CredentialType, true, row.LastFour, row.UpdatedAt ?? row.CreatedAt, row.Version);
    public async Task<List<CredentialStatusDto>> ListAsync()
    {
        RequireOwner();
        var rows = await db.Set<ProviderCredential>().AsNoTracking().Where(x => x.BusinessId == Business).ToListAsync();
        return rows.Select(Status).ToList();
    }
    public async Task<CredentialStatusDto> SaveAsync(string type, CredentialUpdateDto dto)
    {
        RequireOwner();
        if (!AllowedTypes.Contains(type) || string.IsNullOrWhiteSpace(dto.Value) || dto.Value.Length > 4096) throw new ArgumentException("Invalid credential.");
        var row = await db.Set<ProviderCredential>().SingleOrDefaultAsync(x => x.BusinessId == Business && x.CredentialType == type);
        if ((row != null && row.Version != dto.ExpectedVersion) || (row == null && dto.ExpectedVersion.HasValue)) throw new DbUpdateConcurrencyException();
        if (row == null) { row = new ProviderCredential { BusinessId = Business, CredentialType = type }; db.Add(row); }
        var value = dto.Value.Trim();
        row.EncryptedValue = Protector(type).Protect(value);
        // A short secret must never be returned in its entirety.
        row.LastFour = value.Length > 4 ? value[^4..] : "";
        row.UpdatedById = user.UserId ?? throw new UnauthorizedAccessException(); row.UpdatedAt = DateTime.UtcNow; row.Version = Guid.NewGuid();
        db.SecurityAuditLogs.Add(new SecurityAuditLog { BusinessId = Business, UserId = user.UserId, EventType = "ProviderCredentialUpdated", Description = $"Updated {type}; value never logged." });
        await db.SaveChangesAsync(); return Status(row);
    }
    public async Task<string?> ResolveAsync(string credentialType, CancellationToken ct = default)
    {
        if (!AllowedTypes.Contains(credentialType) || !user.BusinessId.HasValue) return null;
        var row = await db.Set<ProviderCredential>().AsNoTracking().SingleOrDefaultAsync(x => x.BusinessId == Business && x.CredentialType == credentialType, ct);
        if (row == null) return null;
        try { return Protector(credentialType).Unprotect(row.EncryptedValue); }
        catch (CryptographicException) { return null; }
    }
}
