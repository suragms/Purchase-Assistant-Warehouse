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

        public AIProviderFactory(IEnumerable<IAIProvider> providers)
        {
            _providers = providers;
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
