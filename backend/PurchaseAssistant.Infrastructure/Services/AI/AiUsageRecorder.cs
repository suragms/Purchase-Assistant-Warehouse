using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
namespace PurchaseAssistant.Infrastructure.Services.AI;
public class AiUsageRecorder(AppDbContext db, ICurrentUserService user) : IAIUsageRecorder
{
    public async Task RecordAsync(AIResponse response, bool escalated, CancellationToken ct)
    {
        if (user.BusinessId is not Guid businessId || businessId == Guid.Empty) return;
        var provider = Enum.TryParse<AIProviderType>(response.Provider, true, out var type) ? type.ToString().ToLowerInvariant() : "none";
        db.AiUsageLogs.Add(new AiUsageLog { BusinessId = businessId, Provider = provider,
            LatencyMs = (int)Math.Clamp(response.LatencyMs, 0, int.MaxValue), Escalated = escalated });
        // No request/content/error/model override is persisted. Token usage/cost are unavailable in the target transport.
        await db.SaveChangesAsync(ct);
    }
}
