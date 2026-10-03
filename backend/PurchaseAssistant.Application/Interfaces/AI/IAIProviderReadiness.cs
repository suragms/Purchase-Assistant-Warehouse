namespace PurchaseAssistant.Application.Interfaces.AI;

/// <summary>
/// Optional configuration signal for providers that require external credentials.
/// Providers without this interface remain usable for tests and local implementations.
/// </summary>
public interface IAIProviderReadiness
{
    bool IsConfigured { get; }
}
