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
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        [OperationalNumeric]
        public decimal QuantityUsed { get; set; }
        [OperationalNumeric]
        public decimal CurrentStock { get; set; }
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

        [Required]
        [Range(0.001, (double)decimal.MaxValue, ErrorMessage = "Quantity used must be greater than 0")]
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
        public decimal AverageCompletionRate { get; set; }
        public List<string> FrequentlySkippedTasks { get; set; } = new();
    }
}