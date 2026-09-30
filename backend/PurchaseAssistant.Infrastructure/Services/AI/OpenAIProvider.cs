using System.Net.Http.Json;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class OpenAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIProvider(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public AIProviderType ProviderType => AIProviderType.OpenAI;

    public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            var payload = new
            {
                model = request.ModelOverride ?? "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = request.SystemPrompt ?? "You are a helpful assistant." },
                    new { role = "user", content = request.Prompt }
                },
                temperature = 0
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            httpRequest.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, ct);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<dynamic>(cancellationToken: ct);
            // Simplified parsing for brevity, in real impl use proper DTOs
            string? content = data?.choices[0].message.content;

            return new AIResponse(
                Success: true,
                Content: content,
                Error: null,
                Provider: ProviderType.ToString(),
                ModelUsed: request.ModelOverride ?? "gpt-4o-mini",
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
                ModelUsed: request.ModelOverride ?? "gpt-4o-mini",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
    }
}
