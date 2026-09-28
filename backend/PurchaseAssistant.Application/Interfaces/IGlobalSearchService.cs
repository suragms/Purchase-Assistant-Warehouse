using PurchaseAssistant.Application.DTOs.Catalog;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IGlobalSearchService
    {
        Task<GlobalSearchResponseDto> SearchAsync(string query, CancellationToken cancellationToken = default);
    }
}
