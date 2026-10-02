using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class AIRoutingService : IAIRoutingService
{
    private readonly IAIProviderFactory _providerFactory;
    private readonly ILogger<AIRoutingService> _logger;
    private readonly AiOptions _aiOptions;
    private readonly IAIUsageRecorder? _usage;

    // Define priority order
    private readonly AIProviderType[] _failoverOrder =
    {
        AIProviderType.OpenRouter,
        AIProviderType.Gemini,
        AIProviderType.Groq,
        AIProviderType.OpenAI,
        AIProviderType.Stub
    };

    public AIRoutingService(IAIProviderFactory providerFactory, ILogger<AIRoutingService> logger, IOptions<AiOptions> aiOptions, IAIUsageRecorder? usage = null)
    {
        _providerFactory = providerFactory;
        _logger = logger;
        _aiOptions = aiOptions.Value;
        _usage = usage;
    }

    public async Task<AIResponse> ExecuteWithFailoverAsync(AIRequest request, CancellationToken ct = default)
    {
        if (!_aiOptions.Enabled)
        {
            return new AIResponse(false, null, "AI_DISABLED", "None", "None", 0);
        }

        var attempts = 0; var watch = System.Diagnostics.Stopwatch.StartNew();
        async Task<AIResponse> Record(AIResponse result) {
            if (_usage != null) try { await _usage.RecordAsync(result with { LatencyMs = watch.ElapsedMilliseconds }, attempts > 1, ct); }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                catch (Exception) { _logger.LogWarning("AI usage metadata could not be recorded"); }
            return result;
        }
        foreach (var providerType in _failoverOrder)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var provider = await _providerFactory.GetProviderAsync(providerType, ct);
                attempts++;
                var response = await provider.SendRequestAsync(request, ct);
                if (response.Success)
                {
                    _logger.LogInformation("AI Request succeeded using {Provider}", providerType);
                    return await Record(response);
                }

                _logger.LogWarning("AI Request failed using {Provider}", providerType);
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation("Provider {Provider} not configured, skipping.", providerType);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (Exception)
            {
                _logger.LogWarning("Provider {Provider} unavailable", providerType);
            }
        }

        return await Record(new AIResponse(
            Success: false,
            Content: null,
            Error: "All AI providers failed",
            Provider: "None",
            ModelUsed: "None",
            LatencyMs: 0
        ));
    }
}
