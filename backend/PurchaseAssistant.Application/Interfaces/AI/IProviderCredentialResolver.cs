namespace PurchaseAssistant.Application.Interfaces.AI;

public interface IProviderCredentialResolver
{
    Task<string?> ResolveAsync(string credentialType, CancellationToken ct = default);
}
