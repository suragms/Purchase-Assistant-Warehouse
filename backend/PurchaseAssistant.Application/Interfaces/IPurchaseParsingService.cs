using PurchaseAssistant.Application.DTOs.Purchase;

namespace PurchaseAssistant.Application.Interfaces;

public interface IPurchaseParsingService
{
    Task<PurchaseIntentCandidateDto> ParseAsync(string prompt, CancellationToken ct = default);
}
