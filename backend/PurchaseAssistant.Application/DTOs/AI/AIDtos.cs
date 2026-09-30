using System.Text.Json.Serialization;

namespace PurchaseAssistant.Application.DTOs.AI;

public record AIRequest(
    string Prompt,
    string? SystemPrompt = null,
    string? ModelOverride = null
);

public record AIResponse(
    bool Success,
    string? Content,
    string? Error,
    string Provider,
    string ModelUsed,
    decimal LatencyMs
);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AIProviderType
{
    OpenRouter,
    Gemini,
    Groq,
    OpenAI,
    Stub
}
