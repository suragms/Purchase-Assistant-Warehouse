using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PurchaseAssistant.Infrastructure.Services.AI
{
    public class AIProviderFactory : IAIProviderFactory
    {
        private readonly IEnumerable<IAIProvider> _providers;
        private readonly IProviderCredentialResolver? _credentials;
        private readonly Func<string, HttpClient>? _clients;

        public AIProviderFactory(IEnumerable<IAIProvider> providers, IProviderCredentialResolver? credentials = null, Func<string, HttpClient>? clients = null)
        {
            _providers = providers;
            _credentials = credentials; _clients = clients;
        }

        public async Task<IAIProvider> GetProviderAsync(AIProviderType type, CancellationToken ct = default)
        {
            var keyType = type switch { AIProviderType.OpenRouter => "openrouter_key", AIProviderType.Gemini => "gemini_key", AIProviderType.Groq => "groq_key", AIProviderType.OpenAI => "openai_key", _ => null };
            var key = keyType != null && _credentials != null ? await _credentials.ResolveAsync(keyType, ct) : null;
            if (string.IsNullOrWhiteSpace(key) || _clients == null) return GetProvider(type);
            return type switch {
                AIProviderType.OpenRouter => new OpenRouterProvider(_clients(nameof(OpenRouterProvider)), key),
                AIProviderType.Gemini => new GeminiProvider(_clients(nameof(GeminiProvider)), key),
                AIProviderType.Groq => new GroqProvider(_clients(nameof(GroqProvider)), key),
                AIProviderType.OpenAI => new OpenAIProvider(_clients(nameof(OpenAIProvider)), key),
                _ => GetProvider(type)
            };
        }

        public IAIProvider GetProvider(AIProviderType providerType)
        {
            var provider = _providers.FirstOrDefault(p => p.ProviderType == providerType);
            if (provider == null)
            {
                throw new NotSupportedException($"Provider {providerType} is not supported or registered.");
            }
            return provider;
        }
    }
}
