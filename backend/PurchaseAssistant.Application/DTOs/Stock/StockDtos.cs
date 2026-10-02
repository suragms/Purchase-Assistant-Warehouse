using PurchaseAssistant.Application.DTOs;
using System;

namespace PurchaseAssistant.Application.DTOs.Stock
{
    public record StockCsvRow(Guid Id, string Name, string Category, string? Subcategory, string Unit,
        [property: OperationalNumeric] decimal Current, [property: OperationalNumeric] decimal Physical,
        [property: OperationalNumeric] decimal Reorder, [property: OperationalNumeric] decimal? Purchased, string Status, string? Supplier, DateTime? LastMovement);
    public class StockItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DefaultUnit { get; set; } = string.Empty;

        [OperationalNumeric]
        public decimal SystemStock { get; set; }
        [OperationalNumeric]
        public decimal PhysicalStock { get; set; }
        [OperationalNumeric]
        public decimal ReservedStock { get; set; }
        [OperationalNumeric]
        public decimal AvailableStock { get; set; }
        [OperationalNumeric]
        public decimal ReorderLevel { get; set; }

        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
    }

    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid CatalogItemId { get; set; }
        public string MovementType { get; set; } = string.Empty;
        [OperationalNumeric]
        public decimal QuantityDelta { get; set; }
        [OperationalNumeric]
        public decimal QuantityBefore { get; set; }
        [OperationalNumeric]
        public decimal QuantityAfter { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AdjustStockRequestDto
    {
        // Internal workflow provenance; API callers cannot supply ledger references.
        [System.Text.Json.Serialization.JsonIgnore]
        public string? ReferenceType { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? ReferenceId { get; set; }
        [OperationalNumeric]
        public decimal QuantityDelta { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public Guid ExpectedVersion { get; set; }
    }

    public class UpdatePhysicalStockRequestDto
    {
        [OperationalNumeric]
        public decimal PhysicalStock { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public Guid ExpectedVersion { get; set; }
    }

    public class ReconcileStockRequestDto
    {
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public Guid ExpectedVersion { get; set; }
    }
}
