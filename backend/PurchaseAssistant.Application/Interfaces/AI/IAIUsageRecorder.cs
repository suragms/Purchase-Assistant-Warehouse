using PurchaseAssistant.Application.DTOs.AI;
namespace PurchaseAssistant.Application.Interfaces.AI;
public interface IAIUsageRecorder
{
    Task RecordAsync(AIResponse response, bool escalated, CancellationToken ct);
}
