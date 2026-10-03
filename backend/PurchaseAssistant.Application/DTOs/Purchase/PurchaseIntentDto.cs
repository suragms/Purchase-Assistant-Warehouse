using PurchaseAssistant.Application.DTOs;
using System.Text.Json.Serialization;

namespace PurchaseAssistant.Application.DTOs.Purchase;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IntentStatus
{
    Success,
    AmbiguousMatch,
    MissingInformation,
    Error
}

public record PurchaseIntentCandidateDto(
    IntentStatus Status,
    string? Message,
    Guid? SupplierId,
    string? SupplierName,
    List<PurchaseIntentItemCandidateDto> Items,
    string? Notes,
    List<string>? Warnings,
    DateTimeOffset? GeneratedAt = null
);

public record PurchaseIntentItemCandidateDto(
    Guid? CatalogItemId,
    string? ItemCode,
    string? CatalogItemName,
    [property: OperationalNumeric] decimal RequestedQuantity,
    string? UnitOfMeasure,
    bool IsAmbiguous,
    List<CatalogItemOptionDto>? Options
);

public record CatalogItemOptionDto(
    Guid CatalogItemId,
    string Name,
    string? Description
);
