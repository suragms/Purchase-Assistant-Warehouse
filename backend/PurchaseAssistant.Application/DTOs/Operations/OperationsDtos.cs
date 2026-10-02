using System;
using System.ComponentModel.DataAnnotations;

namespace PurchaseAssistant.Application.DTOs.Operations
{
    /// <summary>
    /// Checklist slots for daily operations (morning/midday/evening)
    /// </summary>
    public enum ChecklistSlot
    {
        Morning,
        Midday,
        Evening
    }

    /// <summary>
    /// Individual checklist task output
    /// </summary>
    public class ChecklistTaskDto
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByUser { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Today's checklist for all slots
    /// </summary>
    public class ChecklistTodayDto
    {
        public DateTime Date { get; set; }
        public List<ChecklistTaskDto> Morning { get; set; } = new();
        public List<ChecklistTaskDto> Midday { get; set; } = new();
        public List<ChecklistTaskDto> Evening { get; set; } = new();
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        [OperationalNumeric]
        public decimal CompletionPercentage { get; set; }
    }

    /// <summary>
    /// Complete a specific checklist task
    /// </summary>
    public class ChecklistCompleteDto
    {
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Daily usage line item
    /// </summary>
    public class UsageLineDto
    {
        public Guid CatalogItemId { get; set; }
        public Guid ExpectedVersion { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        [OperationalNumeric]
        public decimal OpeningQty { get; set; }
        [OperationalNumeric]
        public decimal PurchasedQty { get; set; }
        [OperationalNumeric]
        public decimal QuantityUsed { get; set; }
        [OperationalNumeric]
        public decimal ClosingQty { get; set; }
        public string? Notes { get; set; }
        public DateTime LoggedAt { get; set; }
        public string? LoggedByUser { get; set; }
    }

    /// <summary>
    /// Today's usage log
    /// </summary>
    public class UsageTodayDto
    {
        public DateTime Date { get; set; }
        public List<UsageLineDto> Lines { get; set; } = new();
        public int TotalItems { get; set; }
        [OperationalNumeric]
        public decimal TotalQuantityUsed { get; set; }
    }

    /// <summary>
    /// Submit new usage line
    /// </summary>
    public class UsageLineCreateDto
    {
        [Required]
        public Guid CatalogItemId { get; set; }
        public Guid ExpectedVersion { get; set; }

        [Required]
        [Range(0, 1000000000)]
        [OperationalNumeric]
        public decimal QuantityUsed { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Submit multiple usage lines for today
    /// </summary>
    public class UsageSubmitDto
    {
        [Required]
        public List<UsageLineCreateDto> Lines { get; set; } = new();
    }

    /// <summary>
    /// Usage summary statistics
    /// </summary>
    public class UsageSummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalItems { get; set; }
        public int MissingItems { get; set; }
        [OperationalNumeric]
        public decimal TotalQuantityUsed { get; set; }
        public List<UsageLineDto> TopItems { get; set; } = new();
        public int LowStockItems { get; set; }
        public int DeadStockItems { get; set; }
        public int FastMovingItems { get; set; }
        public int SlowMovingItems { get; set; }
    }

    /// <summary>
    /// Checklist template definition
    /// </summary>
    public class ChecklistTemplateDto
    {
        public ChecklistSlot Slot { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Checklist summary across dates
    /// </summary>
    public class ChecklistSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public int CompletedDays { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        [OperationalNumeric]
        public decimal AverageCompletionRate { get; set; }
        public List<string> FrequentlySkippedTasks { get; set; } = new();
    }

    public class DailyUsageSnapshotDto : UsageLineDto
    {
        public DateTime Date { get; set; }
    }
    public class StaffTaskCreateDto
    {
        public Guid StaffId { get; set; }
        [Required, StringLength(64)] public string TaskType { get; set; } = "general";
        [StringLength(255)] public string? ReferenceId { get; set; }
    }
    public class StaffTaskActionDto
    {
        public Guid ExpectedVersion { get; set; }
        public bool Rejected { get; set; }
        [StringLength(1000)] public string? CorrectionNote { get; set; }
    }
    public class StaffTaskDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = "";
        public string TaskType { get; set; } = "";
        public string? ReferenceId { get; set; }
        public string Status { get; set; } = "";
        public bool Rejected { get; set; }
        public string? CorrectionNote { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid Version { get; set; }
    }
    public record TaskAssigneeDto(Guid Id, string Name);
    public record StaffPerformanceDto(Guid StaffId, string StaffName, int Total, int Completed, int Pending, int Rejected);
    public class OwnerOperationsDashboardDto
    {
        public DateTime AsOf { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int PendingDamageCount { get; set; }
        public int AiRequestsToday { get; set; }
        public string? BackupLastStatus { get; set; }
        public DateTime? BackupLastAt { get; set; }
        public long? BackupLastSizeBytes { get; set; }
        [FinancialField] public decimal SpendLast7Days { get; set; }
        public List<string> Exceptions { get; set; } = new();
        public List<StaffPerformanceDto> StaffPerformance { get; set; } = new();
    }
}
namespace PurchaseAssistant.Application.DTOs.Operations
{
    public class OperationsReportSummaryDto
    {
        public string Method { get; set; } = "RULE-BASED";
        public List<OperationsReportItemDto> Items { get; set; } = new();
        public Dictionary<string, int> Summary { get; set; } = new();
        public List<OperationsReportItemDto> DeadStock { get; set; } = new();
        public List<OperationsReportItemDto> FastMoving { get; set; } = new();
        public List<OperationsReportItemDto> SlowMoving { get; set; } = new();
        public List<SupplierFrequencyDto> SupplierFrequency { get; set; } = new();
    }
    public class OperationsReportItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string ItemCode { get; set; } = "";
        public string Category { get; set; } = "";
        public string Unit { get; set; } = "";
        public DateTime? LastMovementAt { get; set; }
        public string AgingBucket { get; set; } = "";
        public string InsightKey { get; set; } = "";
        [OperationalNumeric] public decimal CurrentStock { get; set; }
        [OperationalNumeric] public decimal Used7d { get; set; }
        [OperationalNumeric] public decimal Used30d { get; set; }
        public int IdleDays { get; set; }
        public string MovementStatus { get; set; } = "";
    }
    public record SupplierFrequencyDto(Guid SupplierId, string SupplierName, int PurchaseCount);
}
