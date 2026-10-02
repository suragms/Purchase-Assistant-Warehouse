using System;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.Application.DTOs.Purchases;

public class CreateDamageReportDto
{
    public Guid PurchaseOrderId { get; set; }
    public Guid? CatalogItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    [OperationalNumeric] public decimal QtyDamaged { get; set; }
    public string? Unit { get; set; }
    public string DamageType { get; set; } = "damaged";
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class PurchaseDamageReportDto
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    [OperationalNumeric] public decimal QtyDamaged { get; set; }
    public string? Unit { get; set; }
    public string DamageType { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
