using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class OpenRouterProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenRouterProvider(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public AIProviderType ProviderType => AIProviderType.OpenRouter;

    public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            var payload = new
            {
                model = request.ModelOverride ?? "deepseek/deepseek-r1",
                messages = new[]
                {
                    new { role = "user", content = request.Prompt }
                }
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            httpRequest.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, ct);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<dynamic>(cancellationToken: ct);
            string? content = data?.choices[0].message.content;

            return new AIResponse(
                Success: true,
                Content: content,
                Error: null,
                Provider: ProviderType.ToString(),
                ModelUsed: request.ModelOverride ?? "deepseek/deepseek-r1",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
        catch (Exception ex)
        {
            return new AIResponse(
                Success: false,
                Content: null,
                Error: ex.Message,
                Provider: ProviderType.ToString(),
                ModelUsed: request.ModelOverride ?? "deepseek/deepseek-r1",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
    }
}
