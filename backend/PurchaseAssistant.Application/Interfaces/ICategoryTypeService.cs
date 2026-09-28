using PurchaseAssistant.Application.DTOs.Catalog;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface ICategoryTypeService
    {
        Task<List<CategoryTypeDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<CategoryTypeDto> CreateAsync(Guid categoryId, string name, CancellationToken cancellationToken = default);
        Task<CategoryTypeDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
