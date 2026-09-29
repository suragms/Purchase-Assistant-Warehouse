using System;

namespace PurchaseAssistant.Application.DTOs.Stock
{
    public class StockItemDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DefaultUnit { get; set; } = string.Empty;

        public decimal SystemStock { get; set; }
        public decimal PhysicalStock { get; set; }
        public decimal ReservedStock { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal ReorderLevel { get; set; }

        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
    }

    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid CatalogItemId { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public decimal QuantityDelta { get; set; }
        public decimal QuantityBefore { get; set; }
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
        public decimal QuantityDelta { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public Guid ExpectedVersion { get; set; }
    }

    public class UpdatePhysicalStockRequestDto
    {
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
