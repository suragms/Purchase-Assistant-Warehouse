using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Application.Interfaces.AI;

namespace PurchaseAssistant.Application.Interfaces.AI
{
    public interface IAIProviderFactory
    {
        IAIProvider GetProvider(AIProviderType providerType);
        Task<IAIProvider> GetProviderAsync(AIProviderType providerType, CancellationToken ct = default) => Task.FromResult(GetProvider(providerType));
    }
}
