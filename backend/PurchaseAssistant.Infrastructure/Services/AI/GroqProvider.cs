using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace PurchaseAssistant.Infrastructure.Services.AI;

public class GroqProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GroqProvider(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public AIProviderType ProviderType => AIProviderType.Groq;

    public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            var payload = new
            {
                model = "llama3-8b-8192", // Use a default model
                messages = new[]
                {
                    new { role = "system", content = request.SystemPrompt ?? "Extract purchase intent as JSON." },
                    new { role = "user", content = request.Prompt }
                }
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            httpRequest.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(httpRequest, ct);
            response.EnsureSuccessStatusCode();

            using var data = await System.Text.Json.JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            string? content = data.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return new AIResponse(
                Success: !string.IsNullOrWhiteSpace(content),
                Content: content,
                Error: null,
                Provider: ProviderType.ToString(),
                ModelUsed: "llama3-8b-8192",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception)
        {
            return new AIResponse(
                Success: false,
                Content: null,
                Error: "AI_PROVIDER_FAILED",
                Provider: ProviderType.ToString(),
                ModelUsed: "llama3-8b-8192",
                LatencyMs: (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds
            );
        }
    }
}
