using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using System.Net.Http.Json;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class GeminiProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiProvider(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public AIProviderType ProviderType => AIProviderType.Gemini;

    public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            // Gemini API call logic here
            // Note: I will use a dummy endpoint for now based on the prompt instructions to keep it simple and safe.
            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = request.Prompt } } }
                }
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey);
            httpRequest.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, ct);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<dynamic>(cancellationToken: ct);
            // Simplified parsing for brevity
            string? content = data?.candidates[0].content.parts[0].text;

            return new AIResponse(
                Success: true,
                Content: content,
                Error: null,
                Provider: ProviderType.ToString(),
                ModelUsed: "gemini-1.5-flash",
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
                ModelUsed: "gemini-1.5-flash",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
    }
}
