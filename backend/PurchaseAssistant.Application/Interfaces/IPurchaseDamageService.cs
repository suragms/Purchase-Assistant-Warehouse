using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Purchase;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IPurchaseDamageService
    {
        /// <summary>
        /// Retrieves damage reports for a specific purchase order.
        /// </summary>
        Task<List<DamageReportDto>> GetDamageReportsAsync(Guid purchaseOrderId);

        /// <summary>
        /// Creates a new damage report and emits a high-priority notification.
        /// </summary>
        Task<DamageReportDto> CreateDamageReportAsync(Guid purchaseOrderId, CreateDamageReportDto dto);

        /// <summary>
        /// Updates the status of an existing damage report (owner/manager only) and emits an acknowledgment notification.
        /// </summary>
        Task<DamageReportDto> UpdateDamageReportStatusAsync(Guid purchaseOrderId, Guid reportId, UpdateDamageReportStatusDto dto);

        /// <summary>
        /// Gets the total count of pending damage reports across the business.
        /// </summary>
        Task<PendingDamageReportsCountDto> GetPendingCountAsync();
    }
}