using PurchaseAssistant.Application.DTOs.AI;

namespace PurchaseAssistant.Application.Interfaces.AI;

public interface IAIProvider
{
    AIProviderType ProviderType { get; }
    Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default);
}
