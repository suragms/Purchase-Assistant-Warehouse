using System;
using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities
{
    public class StaffTask : TenantEntity
    {
        public Guid StaffId { get; set; }
        public string TaskType { get; set; } = "general";
        public string? ReferenceId { get; set; }
        public string Status { get; set; } = "assigned";
        public bool Rejected { get; set; }
        public string? CorrectionNote { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid Version { get; set; } = Guid.NewGuid();
    }
    /// <summary>
    /// Daily checklist task completion record.
    /// Tracks which staff member completed which task at what time.
    /// </summary>
    public class ChecklistCompletion : TenantEntity
    {
        /// <summary>The date this checklist applies to (YYYY-MM-DD only)</summary>
        public DateOnly Date { get; set; }

        /// <summary>Time slot: morning, midday, evening</summary>
        public string Slot { get; set; } = string.Empty;

        /// <summary>Task identifier matching ChecklistTemplate.Key</summary>
        public string TaskKey { get; set; } = string.Empty;

        /// <summary>When the task was marked complete</summary>
        public DateTime CompletedAt { get; set; }

        /// <summary>User who completed the task</summary>
        public Guid CompletedByUserId { get; set; }
        public User CompletedByUser { get; set; } = null!;

        /// <summary>Optional notes from the user</summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Daily usage log entry for tracking item consumption.
    /// Used for inventory turnover analysis and dead/fast stock reporting.
    /// </summary>
    public class DailyUsageLog : TenantEntity
    {
        /// <summary>The date this usage occurred (YYYY-MM-DD only)</summary>
        public DateOnly Date { get; set; }

        /// <summary>Which catalog item was used</summary>
        public Guid CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;

        /// <summary>Stock level at start of day</summary>
        public decimal OpeningQty { get; set; }

        /// <summary>Amount purchased/received during day</summary>
        public decimal PurchasedQty { get; set; }

        /// <summary>How much was consumed/used</summary>
        public decimal UsedQty { get; set; }

        /// <summary>Stock level at end of day</summary>
        public decimal ClosingQty { get; set; }

        /// <summary>Optional notes about the usage</summary>
        public string? Notes { get; set; }

        /// <summary>User who logged this usage</summary>
        public Guid LoggedByUserId { get; set; }
        public User LoggedByUser { get; set; } = null!;

        /// <summary>When this usage was recorded</summary>
        public DateTime LoggedAt { get; set; }
    }

    /// <summary>
    /// Checklist template defining which tasks should be done in each time slot.
    /// Configurable per business.
    /// </summary>
    public class ChecklistTemplate : TenantEntity
    {
        /// <summary>Time slot: morning, midday, evening</summary>
        public string Slot { get; set; } = string.Empty;

        /// <summary>Unique task identifier within the business</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>Human-readable description of what to do</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Display order within the slot (1, 2, 3...)</summary>
        public int Priority { get; set; }

        /// <summary>Whether this task is currently active</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Pre-aggregated daily operational snapshot for fast reporting.
    /// Materialized from usage logs and checklist completions.
    /// </summary>
    public class DailyOperationSnapshot : TenantEntity
    {
        /// <summary>The date this snapshot represents</summary>
        public DateOnly Date { get; set; }

        /// <summary>Total checklist tasks defined for this day</summary>
        public int TotalChecklistTasks { get; set; }

        /// <summary>Number of tasks actually completed</summary>
        public int CompletedChecklistTasks { get; set; }

        /// <summary>Percentage completion (0-100)</summary>
        public decimal ChecklistCompletionRate { get; set; }

        /// <summary>Total distinct catalog items logged as used</summary>
        public int TotalItemsUsed { get; set; }

        /// <summary>Sum of all usage quantities</summary>
        public decimal TotalQuantityUsed { get; set; }

        /// <summary>Items with zero stock at end of day</summary>
        public int DeadStockItems { get; set; }

        /// <summary>Items with high turnover (usage > average)</summary>
        public int FastMovingItems { get; set; }

        /// <summary>Items with low turnover (usage < average)</summary>
        public int SlowMovingItems { get; set; }

        /// <summary>When this snapshot was last updated</summary>
        public DateTime MaterializedAt { get; set; }
    }
}
