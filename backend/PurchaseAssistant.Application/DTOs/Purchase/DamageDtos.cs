using System;
using System.ComponentModel.DataAnnotations;

namespace PurchaseAssistant.Application.DTOs.Purchase
{
    /// <summary>
    /// Valid damage types verified from reference schema.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum DamageType
    {
        Damaged,    // "damaged"
        Short,      // "short" (short-delivered)
        Missing,    // "missing"
        Returned    // "returned"
    }

    /// <summary>
    /// Valid damage reasons verified from reference schema.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum DamageReason
    {
        TornBag,    // "torn_bag"
        WetDamage,  // "wet_damage"
        WrongItem,  // "wrong_item"
        ShortWeight,// "short_weight"
        Other       // "other"
    }

    /// <summary>
    /// Valid damage status verified from reference schema.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum DamageStatus
    {
        Pending,    // "pending"
        Approved,   // "approved"
        Returned,   // "returned"
        Rejected    // "rejected"
    }

    /// <summary>
    /// Create a new damage report.
    /// </summary>
    public class CreateDamageReportDto
    {
        public string? ItemName { get; set; }

        [Required]
        [Range(0.001, (double)decimal.MaxValue, ErrorMessage = "Damaged quantity must be greater than 0")]
        [OperationalNumeric]
        public decimal QtyDamaged { get; set; }

        public DamageType? DamageType { get; set; }
        public Guid? CatalogItemId { get; set; }

        [MaxLength(32)]
        public string? Unit { get; set; }

        public DamageReason? Reason { get; set; }

        [MaxLength(2000)]
        public string? PhotoUrl { get; set; }

        [MaxLength(4000)]
        public string? Notes { get; set; }

        public bool EmitNotification { get; set; } = true;

        [Range(1, 500)]
        public int? DamagedItemsInBatch { get; set; }
    }

    /// <summary>
    /// Update damage report status (owner/manager only).
    /// </summary>
    public class UpdateDamageReportStatusDto
    {
        [Required]
        public DamageStatus Status { get; set; }

        [MaxLength(4000)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Damage report response DTO.
    /// </summary>
    public class DamageReportDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ReportedBy { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid? CatalogItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        [OperationalNumeric]
        public decimal QtyDamaged { get; set; }
        public string? Unit { get; set; }
        public string DamageType { get; set; } = "damaged";
        public string? Reason { get; set; }
        public string Status { get; set; } = "pending";
        public string? PhotoUrl { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Pending damage reports count for dashboard.
    /// </summary>
    public class PendingDamageReportsCountDto
    {
        public int Count { get; set; }
    }
}