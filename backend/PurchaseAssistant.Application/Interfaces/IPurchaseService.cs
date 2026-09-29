using System;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Catalog;
using PurchaseAssistant.Application.DTOs.Purchase;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<PaginatedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(
            int page, int pageSize, string? search, PurchaseStatus? status, Guid? supplierId);
        Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(Guid id);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(UpsertPurchaseOrderDto dto);
        Task<PurchaseOrderDto> UpdatePurchaseOrderAsync(Guid id, UpsertPurchaseOrderDto dto);
        Task DeletePurchaseOrderAsync(Guid id);
        Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, PurchaseStatus newStatus);
        Task<PurchaseOrderDto> ReceiveItemsAsync(Guid id, ReceivePurchaseDto dto);
    }
}
