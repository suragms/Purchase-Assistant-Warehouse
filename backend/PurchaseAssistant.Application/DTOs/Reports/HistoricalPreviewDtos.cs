using PurchaseAssistant.Application.DTOs;

namespace PurchaseAssistant.Application.DTOs.Reports;

// In-memory structures, never entities. Decimal source values stay strings until validated.
public record HistoricalValue(string State, string? Value = null, string? OriginalAllowedValue = null, string SourceCell = "synthetic-cell");
public record HistoricalProvenance(string SourceIdentifier, string SourceKind, string ImportIdentifier,
    string RowIdentifier, string SourceRowIdentifier, Guid Actor, string RecordedAt,
    string? SourceTimestamp = null, string? PriorRevision = null, string? CorrectionReason = null);
public record HistoricalRow(string Case, Guid BusinessId, Guid? ItemId, string? ItemCode, string? Barcode,
    string SourceItemIdentifier, Guid? PurchaseId, Guid? LineId, Guid? SupplierId, string EffectiveDate,
    string DateMeaning, string Currency, string RateBasis, string OriginalStockUnit, string QuantityScope,
    string SourceQuantity, HistoricalProvenance Provenance, IReadOnlyDictionary<string, HistoricalValue> Fields,
    string? SellingCostAlias = null);
public record HistoricalSourceMap(string SourceIdentifier, Guid BusinessId, string SourceItemIdentifier, Guid ItemId);
public record HistoricalPreviewInput(string SchemaVersion, bool Synthetic, Guid BusinessId,
    IReadOnlyList<HistoricalRow> Rows, IReadOnlyList<HistoricalSourceMap> SourceMap);
public record HistoricalTarget(Guid BusinessId, Guid ItemId, string? ItemCode, string? Barcode,
    Guid PurchaseId, Guid LineId, Guid SupplierId, string CurrentName, string StockUnit, string LineUnit,
    [property: OperationalNumeric] decimal CurrentStock, [property: OperationalNumeric] decimal PhysicalStock,
    [property: OperationalNumeric] decimal? KgPerUnit);
public record HistoricalFieldResult(string Field, string State, string Outcome, string? ProposedValue,
    string ReasonCode, string Message, string? SourceCell, string? OriginalAllowedValue);
public record HistoricalRowResult(string RowIdentifier, string Case, string Match, string Outcome, bool Duplicate,
    Guid? MatchedItemId, IReadOnlyList<HistoricalFieldResult> Fields, HistoricalProvenance? Provenance,
    IReadOnlyDictionary<string, string?> UnchangedCurrentValues, IReadOnlyList<string> Reasons);
public record HistoricalPreviewSummary([property: OperationalNumeric] int TotalRows,
    [property: OperationalNumeric] int ValidRows, [property: OperationalNumeric] int WarningRows,
    [property: OperationalNumeric] int RejectedRows, [property: OperationalNumeric] int AmbiguousRows,
    [property: OperationalNumeric] int NotFoundRows, [property: OperationalNumeric] int OutOfScopeRows,
    [property: OperationalNumeric] int DuplicateRows);
public record HistoricalPreviewResult(Guid BusinessId, string SchemaVersion, string Label, bool Synthetic,
    bool WritesPerformed, bool PersistenceAvailable, bool ConfirmationAvailable, HistoricalPreviewSummary Summary,
    IReadOnlyList<HistoricalRowResult> Rows, IReadOnlyList<string> UnchangedAreas);
