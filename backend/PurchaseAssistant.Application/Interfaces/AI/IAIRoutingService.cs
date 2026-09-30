using PurchaseAssistant.Application.DTOs.AI;

namespace PurchaseAssistant.Application.Interfaces.AI;

public interface IAIRoutingService
{
    Task<AIResponse> ExecuteWithFailoverAsync(AIRequest request, CancellationToken ct = default);
}
