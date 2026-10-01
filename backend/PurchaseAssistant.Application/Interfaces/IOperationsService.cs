using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Operations;
using PurchaseAssistant.Application.DTOs.Reports;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IOperationsService
    {
        // Checklist routines
        Task<ChecklistTodayDto> GetTodayChecklistAsync();
        Task CompleteChecklistTaskAsync(ChecklistSlot slot, string taskKey, ChecklistCompleteDto dto);
        Task<List<ChecklistTemplateDto>> GetChecklistTemplatesAsync();
        Task<List<ChecklistTemplateDto>> UpdateChecklistTemplatesAsync(List<ChecklistTemplateDto> templates);
        Task<ChecklistSummaryDto> GetChecklistSummaryAsync(DateTime? startDate, DateTime? endDate);

        // Usage Routines
        Task<UsageTodayDto> GetTodayUsageAsync();
        Task<UsageSummaryDto> SubmitUsageAsync(UsageSubmitDto dto);
        Task<UsageSummaryDto> GetUsageSummaryAsync(DateTime? date);

        // Daily Snapshot Materialization
        Task<int> MaterializeSnapshotsAsync(DateTime? forDate = null);

        // Report specific aggregations matching reference implementations
        Task<OperationsReportSummaryDto> GetOperationsReportSummaryAsync();
    }
}