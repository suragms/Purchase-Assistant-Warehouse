using Microsoft.Extensions.Logging;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class AIRoutingService : IAIRoutingService
{
    private readonly IEnumerable<IAIProvider> _providers;
    private readonly ILogger<AIRoutingService> _logger;

    // Define priority order
    private readonly AIProviderType[] _failoverOrder =
    {
        AIProviderType.OpenRouter,
        AIProviderType.Gemini,
        AIProviderType.Groq,
        AIProviderType.OpenAI,
        AIProviderType.Stub
    };

    public AIRoutingService(IEnumerable<IAIProvider> providers, ILogger<AIRoutingService> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<AIResponse> ExecuteWithFailoverAsync(AIRequest request, CancellationToken ct = default)
    {
        foreach (var providerType in _failoverOrder)
        {
            var provider = _providers.FirstOrDefault(p => p.ProviderType == providerType);
            if (provider == null) continue;

            try
            {
                var response = await provider.SendRequestAsync(request, ct);
                if (response.Success)
                {
                    _logger.LogInformation("AI Request succeeded using {Provider}", providerType);
                    return response;
                }

                _logger.LogWarning("AI Request failed using {Provider}: {Error}", providerType, response.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {Provider} threw an exception", providerType);
            }
        }

        return new AIResponse(
            Success: false,
            Content: null,
            Error: "All AI providers failed",
            Provider: "None",
            ModelUsed: "None",
            LatencyMs: 0
        );
    }
}
