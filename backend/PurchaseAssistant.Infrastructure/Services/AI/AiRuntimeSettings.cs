using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using System.Collections.Concurrent;
using System.Text.Json;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public record AiProviderPolicy
{
    public bool Enabled { get; init; } = true;
    public string[] ProviderOrder { get; init; } = ["OpenRouter", "Gemini", "Groq", "OpenAI"];
    public Dictionary<string, string> Models { get; init; } = [];
    public int TimeoutSeconds { get; init; } = 8;
    public int Retries { get; init; } = 0;
    public Guid Version { get; init; }
    public void Validate()
    {
        var allowed = new[] { "OpenRouter", "Gemini", "Groq", "OpenAI" };
        if (ProviderOrder == null || ProviderOrder.Length is < 1 or > 4 || ProviderOrder.Distinct().Count() != ProviderOrder.Length || ProviderOrder.Any(x => !allowed.Contains(x))
            || Models == null || Models.Count > 4 || Models.Any(x => !allowed.Contains(x.Key) || string.IsNullOrWhiteSpace(x.Value) || x.Value.Length > 128 || !System.Text.RegularExpressions.Regex.IsMatch(x.Value, "^[a-zA-Z0-9._:/-]+$"))
            || TimeoutSeconds is < 1 or > 20 || Retries is < 0 or > 1) throw new ArgumentException("Choose supported providers, valid model names, a timeout of 1–20 seconds and at most one retry.");
    }
}
public class AiRuntimeSettings(AppDbContext db, ICurrentUserService user)
{
    public Guid BusinessId => user.BusinessId ?? throw new UnauthorizedAccessException();
    public async Task<AiProviderPolicy> GetAsync(CancellationToken ct = default)
    {
        var business = await db.Businesses.AsNoTracking().SingleOrDefaultAsync(x => x.Id == BusinessId && x.IsActive, ct) ?? throw new KeyNotFoundException();
        var policy = string.IsNullOrWhiteSpace(business.AiSettingsJson) ? new() : JsonSerializer.Deserialize<AiProviderPolicy>(business.AiSettingsJson) ?? new AiProviderPolicy();
        policy.Validate(); return policy with { Version = business.Version };
    }
    public async Task<AiProviderPolicy> SaveAsync(AiProviderPolicy policy, CancellationToken ct)
    {
        policy.Validate();
        var business = await db.Businesses.SingleOrDefaultAsync(x => x.Id == BusinessId && x.IsActive, ct) ?? throw new KeyNotFoundException();
        if (policy.Version != business.Version) throw new DbUpdateConcurrencyException("Provider settings changed. Reload before saving.");
        business.Version = Guid.NewGuid(); business.AiSettingsJson = JsonSerializer.Serialize(policy with { Version = Guid.Empty });
        db.SecurityAuditLogs.Add(new() { BusinessId = BusinessId, UserId = user.UserId, EventType = "AiProviderSettingsUpdated", Description = $"Business:{BusinessId}", MetadataJson = business.AiSettingsJson });
        await db.SaveChangesAsync(ct); return policy with { Version = business.Version };
    }
}
public class AiCircuitBreaker(TimeProvider clock)
{
    private record State(int Failures, DateTimeOffset OpenUntil);
    private readonly ConcurrentDictionary<string, State> states = new();
    public bool IsOpen(string key) => states.TryGetValue(key, out var state) && state.OpenUntil > clock.GetUtcNow();
    public void Success(string key) => states.TryRemove(key, out _);
    public void Failure(string key)
    {
        var now = clock.GetUtcNow();
        if (states.Count > 10000) foreach (var pair in states.Where(x => x.Value.OpenUntil < now.AddMinutes(-5))) states.TryRemove(pair.Key, out _);
        states.AddOrUpdate(key, new State(1, now), (_, old) => { var count = old.OpenUntil < now.AddMinutes(-5) ? 1 : old.Failures + 1; return new State(count, count >= 3 ? now.AddSeconds(60) : now); });
    }
}
