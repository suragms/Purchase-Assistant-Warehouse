using PurchaseAssistant.Application.DTOs.Catalog;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SupplierDto> CreateAsync(SupplierDto dto, CancellationToken cancellationToken = default);
        Task<SupplierDto> UpdateAsync(Guid id, SupplierDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
