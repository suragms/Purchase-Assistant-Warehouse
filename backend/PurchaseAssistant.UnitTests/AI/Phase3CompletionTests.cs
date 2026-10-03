using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Infrastructure.Services.AI;
using PurchaseAssistant.ML;
using Xunit;

namespace PurchaseAssistant.UnitTests.AI;

public partial class PurchaseIntentEndpointTests
{
    [Theory] [InlineData(Role.Owner)] [InlineData(Role.Staff)]
    public async Task PasswordChangeRequiresCurrentSecretRevokesEverySessionAndAudits(Role role)
    {
        using var factory = new Factory { MemberRole = role }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>(); (await db.Users.SingleAsync()).PasswordHash = hasher.HashPassword("old-password-123"); await db.SaveChangesAsync(); }
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/settings/password", new { currentPassword = "wrong", newPassword = "new-password-123" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/settings/password", new { currentPassword = "old-password-123", newPassword = "short" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/v1/settings/password", new { currentPassword = "old-password-123", newPassword = "new-password-123" })).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/settings/profile")).StatusCode);
        using var check = factory.Services.CreateScope(); var saved = check.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.True(check.ServiceProvider.GetRequiredService<IPasswordHasher>().VerifyPassword("new-password-123", (await saved.Users.SingleAsync()).PasswordHash));
        Assert.False(await saved.RefreshTokens.AnyAsync(x => x.RevokedAt == null));
        var audit = await saved.SecurityAuditLogs.IgnoreQueryFilters().SingleAsync(x => x.EventType == "PasswordChanged"); Assert.DoesNotContain("password-123", audit.Description);
    }
    [Theory] [InlineData(Role.Staff)] [InlineData(Role.Manager)] [InlineData(Role.Admin)]
    public async Task AuditRequiresOwnerAndStockReadsRequireCurrentPermission(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = "catalog.view" }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/v1/audit")).StatusCode);
        foreach (var path in new[] { "/operations/usage/today", "/operations/usage/summary", "/operations/snapshots", "/operations/reports/summary" })
            Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/v1" + path)).StatusCode);
    }
    [Fact] public async Task ProfileMutationIsAuditedAndAuditHistoryIsTenantScopedAndImmutable()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/v1/settings/profile", new { name = "Updated owner" })).StatusCode);
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.SecurityAuditLogs.Add(new() { BusinessId = Guid.NewGuid(), EventType = "ForeignSecret", Description = "Must not leak" }); await db.SaveChangesAsync(); }
        var history = await client.GetStringAsync("/api/v1/audit"); Assert.Contains("UserModified", history); Assert.Contains("Updated owner", history); Assert.DoesNotContain("ForeignSecret", history);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/audit?pageSize=100000")).StatusCode);
        using var check = factory.Services.CreateScope(); var dbCheck = check.ServiceProvider.GetRequiredService<AppDbContext>(); var row = await dbCheck.SecurityAuditLogs.IgnoreQueryFilters().FirstAsync(); row.Description = "tampered";
        await Assert.ThrowsAsync<InvalidOperationException>(() => dbCheck.SaveChangesAsync());
    }
    [Theory] [InlineData(Role.Staff, false)] [InlineData(Role.Owner, true)]
    public async Task ProviderSettingsEnforceRoleValidationAndOptimisticVersion(Role role, bool allowed)
    {
        using var factory = new Factory { MemberRole = role }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("providers.manage", true));
        var response = await client.GetAsync("/api/v1/settings/ai"); Assert.Equal(allowed ? HttpStatusCode.OK : HttpStatusCode.Forbidden, response.StatusCode); if (!allowed) return;
        var policy = (await response.Content.ReadFromJsonAsync<AiProviderPolicy>())!;
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PutAsJsonAsync("/api/v1/settings/ai", policy with { ProviderOrder = ["untrusted-host"] })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/v1/settings/ai", policy with { Enabled = false, Models = new() { ["OpenAI"] = "test-model" } })).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync("/api/v1/settings/ai", policy)).StatusCode);
        Assert.False((await client.GetFromJsonAsync<AiProviderPolicy>("/api/v1/settings/ai"))!.Enabled);
    }
    [Theory] [InlineData(Role.Staff)] [InlineData(Role.Owner)]
    public async Task MlSuccessfulForecastKeepsOperationalValuesForStaffAndHandlesCorruption(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = "stock.view" }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        var now = DateTime.UtcNow; var today = DateOnly.FromDateTime(now); var item = new CatalogItem { BusinessId = BusinessId, Name = "Forecast item", CurrentStock = 10, ReorderLevel = 5 };
        var rows = Enumerable.Range(0, 180).Select(i => { var d = today.AddDays(i - 180); return new UsageObservation(d, 20 + i * .15 + 2 * (int)d.DayOfWeek, true, d.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc)); }).ToList();
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.Add(item); db.DailyUsageLogs.AddRange(rows.Select(x => new DailyUsageLog { BusinessId = BusinessId, CatalogItemId = item.Id, Date = x.Date, UsedQty = (decimal)x.Quantity!.Value, IsConfirmed = true, LoggedAt = x.RecordedAt })); await db.SaveChangesAsync(); }
        var missing = await client.GetFromJsonAsync<MlAnalysis>($"/api/v1/ml/items/{item.Id}"); Assert.Equal("model_missing", missing!.Status);
        var store = factory.Services.GetRequiredService<ArtifactStore>(); var artifact = ForecastModel.Train(BusinessId, item.Id, item.DefaultUnit, UsageData.Prepare(rows, today, now), now); await store.SaveAsync(artifact);
        var response = await client.GetAsync($"/api/v1/ml/items/{item.Id}?horizon=14"); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = (await response.Content.ReadFromJsonAsync<MlAnalysis>())!; Assert.Equal("ready", result.Status); Assert.Equal(14, result.Forecast.Count); Assert.True(result.Forecast[0].Quantity > 0); Assert.Equal(10, result.CurrentStock); Assert.True(result.Reorder!.Quantity > 0);
        await File.WriteAllTextAsync(Path.Combine(factory.BackupDirectory, "models", BusinessId.ToString("N"), item.Id.ToString("N") + ".json"), "corrupt");
        Assert.Equal("model_unavailable", (await client.GetFromJsonAsync<MlAnalysis>($"/api/v1/ml/items/{item.Id}"))!.Status);
    }
}
