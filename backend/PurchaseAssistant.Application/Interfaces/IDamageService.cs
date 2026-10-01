using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Purchases;

namespace PurchaseAssistant.Application.Interfaces;

public interface IDamageService
{
    Task<IEnumerable<PurchaseDamageReportDto>> GetReportsAsync(Guid purchaseOrderId);
    Task<PurchaseDamageReportDto> CreateReportAsync(CreateDamageReportDto reportDto);
    Task<bool> ApproveReportAsync(Guid reportId);
}
