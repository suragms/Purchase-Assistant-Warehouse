using System.Net;
using System.Text;
using System.Text.Json;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using PurchaseAssistant.Infrastructure.Services.AI;

namespace PurchaseAssistant.UnitTests.AI;

public class ProviderTransportTests
{
    private sealed class Handler(string json, HttpStatusCode status = HttpStatusCode.OK) : HttpMessageHandler
    {
        public string? RequestBody { get; private set; }
        public Uri? Uri { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            RequestBody = await request.Content!.ReadAsStringAsync(ct); Uri = request.RequestUri;
            return new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }
    }
    [Theory]
    [InlineData(AIProviderType.OpenAI)]
    [InlineData(AIProviderType.Groq)]
    [InlineData(AIProviderType.OpenRouter)]
    [InlineData(AIProviderType.Gemini)]
    public async Task ParsesProviderEnvelope_AndSendsSystemInstructions(AIProviderType type)
    {
        const string candidate = "{\"Items\":[]}";
        var json = type == AIProviderType.Gemini
            ? JsonSerializer.Serialize(new { candidates = new[] { new { content = new { parts = new[] { new { text = candidate } } } } } })
            : JsonSerializer.Serialize(new { choices = new[] { new { message = new { content = candidate } } } });
        var handler = new Handler(json); using var client = new HttpClient(handler);
        IAIProvider provider = type switch { AIProviderType.OpenAI => new OpenAIProvider(client, "test-key"),
            AIProviderType.Groq => new GroqProvider(client, "test-key"), AIProviderType.OpenRouter => new OpenRouterProvider(client, "test-key"),
            _ => new GeminiProvider(client, "test-key") };
        var response = await provider.SendRequestAsync(new("Buy rice", "Return structured JSON"));
        Assert.True(response.Success); Assert.Equal(candidate, response.Content);
        Assert.Contains("Return structured JSON", handler.RequestBody);
        Assert.DoesNotContain("test-key", handler.Uri!.ToString());
    }
}
