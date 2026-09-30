using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class StubAIProvider : IAIProvider
{
    public AIProviderType ProviderType => AIProviderType.Stub;

    public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        await Task.Delay(100, ct); // Simulate latency

        return new AIResponse(
            Success: true,
            Content: "{\"status\": \"stubbed_success\"}",
            Error: null,
            Provider: ProviderType.ToString(),
            ModelUsed: "stub-model",
            LatencyMs: 100
        );
    }
}
