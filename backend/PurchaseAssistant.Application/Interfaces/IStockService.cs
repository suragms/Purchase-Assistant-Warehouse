using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Stock;
using System;
using System.Threading.Tasks;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IStockService
    {
        Task<List<StockCsvRow>> GetCsvRowsAsync(string filter, string? search, DateTime? start, DateTime? end, Guid[]? ids, CancellationToken ct, Guid? categoryId = null, Guid? supplierId = null, string? severity = null);
        Task<PaginatedResult<StockItemDto>> GetStockItemsAsync(int page, int pageSize, string? search, bool? lowStockOnly, bool? outOfStockOnly, Guid? categoryId = null, Guid? supplierId = null, string? severity = null);
        Task<StockItemDto> GetStockDetailAsync(Guid itemId);
        Task<PaginatedResult<StockMovementDto>> GetItemActivityAsync(Guid itemId, int page, int pageSize);

        Task<StockItemDto> AdjustStockAsync(Guid itemId, AdjustStockRequestDto request);
        Task<StockItemDto> UpdatePhysicalStockAsync(Guid itemId, UpdatePhysicalStockRequestDto request);
        Task<StockItemDto> ReconcileStockAsync(Guid itemId, ReconcileStockRequestDto request);
    }
}
