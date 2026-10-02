using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class StubAIProvider : IAIProvider
{
    public AIProviderType ProviderType => AIProviderType.Stub;

    public Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(new AIResponse(
            Success: false,
            Content: null,
            Error: "AI_NOT_CONFIGURED",
            Provider: ProviderType.ToString(),
            ModelUsed: "None",
            LatencyMs: 0
        ));
    }
}
