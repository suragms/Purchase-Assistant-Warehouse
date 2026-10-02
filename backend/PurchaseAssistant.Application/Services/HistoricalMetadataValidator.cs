using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Interfaces;

namespace PurchaseAssistant.Application.Services;

// Pure validator: no repository, DbContext, StockService, transaction, audit or persistence dependency.
public static class HistoricalMetadataValidator
{
    public const string SchemaVersion = "historical-metadata-v1";
    public static readonly string[] FieldNames = ["openingStock", "businessDate", "sellingRate", "historicalName", "historicalUnit", "kgPerUnit", "totalWeight", "normalizedQuantity"];
    private static readonly string[] States = ["KNOWN", "UNKNOWN", "NOT_APPLICABLE", "NOT_CAPTURED"];
    private static readonly string[] Units = ["BAG", "BG", "BGS", "SACK", "BOX", "TIN", "KG", "KILO", "كيلو", "LITRE", "LITER", "LTR", "PCS", "PC", "PIECE", "OTHER"];
    private static bool Id(string? value) => value is { Length: > 0 and <= 100 } && Regex.IsMatch(value, @"\A[A-Za-z0-9][A-Za-z0-9_.:-]*\z");
    private static bool Date(string? value) => value?.Length == 10 && DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    private static bool Timestamp(string? value) => value is { Length: <= 40 } && Regex.IsMatch(value, @"T\d{2}:\d{2}:\d{2}(\.\d{1,7})?(Z|[+-]\d{2}:\d{2})\z") && DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    private static bool Number(string? value, int places, out decimal number, decimal? maximum = null)
    {
        number = 0;
        return value is { Length: > 0 and <= 24 } && Regex.IsMatch(value, @"\A[0-9]+(\.[0-9]{1," + places + @"})?\z") &&
            decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number) && number <= (maximum ?? (places == 2 ? 9999999999.99m : 999999999.999m));
    }
    private static string Unit(string? value) => value?.Trim().ToUpperInvariant() ?? "";
    private static string? Decimal(decimal? value) => value?.ToString(CultureInfo.InvariantCulture);

    public static HistoricalPreviewResult Preview(HistoricalPreviewInput input, IReadOnlyList<HistoricalTarget> targets, ICurrentUserService user)
    {
        // Existing policies grant Owner/SuperAdmin implicit tenant permissions. Forged claims cannot bypass the role gate.
        if (user.UserId is null || user.UserId == Guid.Empty || user.BusinessId is null || user.BusinessId == Guid.Empty || user.Role is not ("Owner" or "SuperAdmin"))
            throw new UnauthorizedAccessException("Historical preview requires a scoped owner or SuperAdmin.");
        if (!input.Synthetic || input.SchemaVersion != SchemaVersion || input.BusinessId != user.BusinessId || input.BusinessId == Guid.Empty ||
            input.Rows.Count is < 1 or > 1000 || JsonSerializer.SerializeToUtf8Bytes(input).Length > 1048576)
            throw new ArgumentException("A scoped synthetic historical-metadata-v1 batch is required (up to 1 MiB / 1000 rows).");

        var results = new List<HistoricalRowResult>();
        var duplicateRows = input.Rows.GroupBy(x => (x.Provenance.ImportIdentifier, x.Provenance.RowIdentifier)).Where(x => x.Count() > 1).SelectMany(x => x).ToHashSet();
        var duplicateSources = input.Rows.GroupBy(x => (x.Provenance.SourceIdentifier, x.Provenance.SourceRowIdentifier)).Where(x => x.Count() > 1).SelectMany(x => x).ToHashSet();
        foreach (var row in input.Rows)
        {
            var reasons = new List<string>();
            var duplicate = duplicateRows.Contains(row) || duplicateSources.Contains(row);
            // Reject ALL duplicate entries; never select a winner or suggest a write twice.
            if (duplicate) reasons.Add("DUPLICATE_PROVENANCE");
            var provenanceValid = Provenance(row, input, user, reasons);
            var (match, target) = Match(row, input, targets, user.BusinessId.Value);
            if (match != "MATCHED") reasons.Add(match);
            var fields = FieldNames.Select(name => Field(name, row, target)).ToList();
            if (row.Fields.Keys.Any(key => !FieldNames.Contains(key))) reasons.Add("UNSUPPORTED_FIELD");
            // A blocked row cannot propose values against an unresolved target or invalid provenance.
            if (!provenanceValid || duplicate || reasons.Contains("UNSUPPORTED_FIELD") || match != "MATCHED")
                fields = fields.Select(x => x with { ProposedValue = null }).ToList();
            var outcome = !provenanceValid || duplicate ? "REJECTED" : match == "AMBIGUOUS" ? "AMBIGUOUS" : match == "NOT_FOUND" ? "NOT_FOUND" :
                match is "INVALID" or "OUT_OF_SCOPE" || reasons.Contains("UNSUPPORTED_FIELD") || fields.Any(x => x.Outcome == "REJECTED") ? "REJECTED" :
                fields.Any(x => x.Outcome == "WARNING") ? "WARNING" : "VALID";
            results.Add(new(Id(row.Provenance.RowIdentifier) ? row.Provenance.RowIdentifier : "invalid-row", row.Case, match, outcome, duplicate,
                target?.ItemId, fields, provenanceValid ? row.Provenance : null,
                target is null ? new Dictionary<string, string?>() : new Dictionary<string, string?> {
                    ["catalogName"] = target.CurrentName, ["currentStock"] = Decimal(target.CurrentStock), ["physicalStock"] = Decimal(target.PhysicalStock),
                    ["stockUnit"] = target.StockUnit, ["lineUnit"] = target.LineUnit, ["kgPerUnit"] = Decimal(target.KgPerUnit) }, reasons));
        }
        int Count(string outcome) => results.Count(x => x.Outcome == outcome);
        return new(input.BusinessId, SchemaVersion, "Preview only — no data will be saved.", true, false, false, false,
            new(results.Count, Count("VALID"), Count("WARNING"), Count("REJECTED"), Count("AMBIGUOUS"), Count("NOT_FOUND"), results.Count(x => x.Match == "OUT_OF_SCOPE"), results.Count(x => x.Duplicate)),
            results, ["Current and physical stock", "Stock movement ledger", "Catalog names and units", "Purchases and purchase lines", "Suppliers and supplier item prices", "Financial totals", "Audit and history records"]);
    }

    private static bool Provenance(HistoricalRow row, HistoricalPreviewInput input, ICurrentUserService user, List<string> reasons)
    {
        var p = row.Provenance;
        if (!Id(p.SourceIdentifier)) reasons.Add("MISSING_OR_INVALID_SOURCE");
        if (!Id(p.ImportIdentifier)) reasons.Add("MISSING_OR_INVALID_IMPORT_IDENTIFIER");
        if (!Id(p.RowIdentifier) || !Id(p.SourceRowIdentifier)) reasons.Add("MISSING_OR_INVALID_ROW_IDENTIFIER");
        if (p.Actor == Guid.Empty || p.Actor != user.UserId) reasons.Add("MISSING_OR_CONFLICTING_ACTOR");
        if (p.SourceKind != "SyntheticFixture") reasons.Add("UNTRUSTED_SOURCE_KIND");
        if (!Timestamp(p.RecordedAt) || (p.SourceTimestamp != null && !Timestamp(p.SourceTimestamp))) reasons.Add("INVALID_SOURCE_TIMESTAMP");
        if (p.PriorRevision != null && (!Id(p.PriorRevision) || string.IsNullOrWhiteSpace(p.CorrectionReason) || p.CorrectionReason.Length > 512)) reasons.Add("CORRECTION_REASON_REQUIRED");
        if (p.PriorRevision == null && p.CorrectionReason != null) reasons.Add("CONFLICTING_CORRECTION_PROVENANCE");
        var maps = input.SourceMap.Where(x => x.SourceIdentifier == p.SourceIdentifier).ToArray();
        if (maps.Length == 0) reasons.Add("UNKNOWN_SOURCE");
        if (maps.Any(x => x.BusinessId != input.BusinessId)) reasons.Add("SOURCE_TENANT_MISMATCH");
        return !reasons.Any(x => x != "DUPLICATE_PROVENANCE");
    }

    private static (string Match, HistoricalTarget? Target) Match(HistoricalRow row, HistoricalPreviewInput input, IReadOnlyList<HistoricalTarget> targets, Guid business)
    {
        if (row.BusinessId != business || targets.Any(x => x.BusinessId != business &&
            (x.ItemId == row.ItemId || x.SupplierId == row.SupplierId || x.PurchaseId == row.PurchaseId || x.LineId == row.LineId))) return ("OUT_OF_SCOPE", null);
        if (!Id(row.SourceItemIdentifier) || row.ItemId == Guid.Empty || row.LineId == Guid.Empty || row.PurchaseId == Guid.Empty || row.SupplierId == Guid.Empty ||
            row.ItemCode is { Length: > 128 } || row.Barcode is { Length: > 128 } || row.ItemCode?.Any(char.IsControl) == true || row.Barcode?.Any(char.IsControl) == true)
            return ("INVALID", null);
        var maps = input.SourceMap.Where(x => x.SourceIdentifier == row.Provenance.SourceIdentifier && x.SourceItemIdentifier == row.SourceItemIdentifier).ToArray();
        if (maps.Any(x => x.BusinessId != business || targets.Any(t => t.BusinessId != business && t.ItemId == x.ItemId))) return ("OUT_OF_SCOPE", null);
        var scoped = targets.Where(x => x.BusinessId == business).ToArray();
        var keys = new List<Guid[]>();
        if (row.ItemId != null) keys.Add(scoped.Where(x => x.ItemId == row.ItemId).Select(x => x.ItemId).Distinct().ToArray());
        if (!string.IsNullOrWhiteSpace(row.ItemCode)) keys.Add(scoped.Where(x => x.ItemCode == row.ItemCode).Select(x => x.ItemId).Distinct().ToArray());
        if (!string.IsNullOrWhiteSpace(row.Barcode)) keys.Add(scoped.Where(x => x.Barcode == row.Barcode).Select(x => x.ItemId).Distinct().ToArray());
        keys.Add(maps.Select(x => x.ItemId).Distinct().ToArray());
        if (keys.Any(x => x.Length > 1) || keys.Where(x => x.Length == 1).Select(x => x[0]).Distinct().Count() > 1) return ("AMBIGUOUS", null);
        if (keys.Any(x => x.Length == 0)) return ("NOT_FOUND", null);
        var candidates = scoped.Where(x => x.ItemId == keys[0][0] && x.LineId == row.LineId && x.PurchaseId == row.PurchaseId && x.SupplierId == row.SupplierId).ToArray();
        return candidates.Length switch { 0 => ("NOT_FOUND", null), 1 => ("MATCHED", candidates[0]), _ => ("AMBIGUOUS", null) };
    }

    private static HistoricalFieldResult Field(string name, HistoricalRow row, HistoricalTarget? target)
    {
        var value = row.Fields.GetValueOrDefault(name) ?? new HistoricalValue("UNKNOWN");
        HistoricalFieldResult Result(string outcome, string reason, string message, string? proposed = null) =>
            new(name, value.State, outcome, proposed, reason, message, Id(value.SourceCell) ? value.SourceCell : null,
                outcome == "VALID" && value.State == "KNOWN" ? value.OriginalAllowedValue : null);
        HistoricalFieldResult Reject(string reason, string message) => Result("REJECTED", reason, message);
        if (!States.Contains(value.State)) return Reject("INVALID_STATE", "An explicit semantic state is required.");
        if (value.State != "KNOWN")
        {
            if (value.Value != null || value.OriginalAllowedValue != null) return Reject("STATE_VALUE_CONFLICT", "An absent state must have a null value.");
            var optional = name is "sellingRate" or "openingStock" or "kgPerUnit" or "totalWeight" or "normalizedQuantity";
            if (value.State == "NOT_CAPTURED" && !optional) return Reject("REQUIRED_SOURCE_FACT", "This source fact requires trusted evidence or UNKNOWN.");
            if (value.State == "NOT_APPLICABLE" && !(name is "kgPerUnit" or "totalWeight" &&
                row.Fields.GetValueOrDefault("historicalUnit") is { State: "KNOWN" } u && Unit(u.Value) is "BOX" or "TIN" or "PCS" or "PC" or "PIECE"))
                return Reject("INVALID_NOT_APPLICABLE", "Only explicit count-only source geometry can be not applicable.");
            return Result(value.State == "UNKNOWN" ? "WARNING" : "VALID", value.State, "Remains null; no historical value is inferred.");
        }
        if (string.IsNullOrWhiteSpace(value.Value) || value.Value != value.OriginalAllowedValue || !Id(value.SourceCell))
            return Reject("MISSING_OR_CONFLICTING_ORIGINAL_VALUE", "KNOWN requires the exact original allowed value and source cell.");
        if (!Date(row.EffectiveDate)) return Reject("INVALID_EFFECTIVE_DATE", "A source calendar effective date is required.");
        var raw = value.Value;
        if (name == "businessDate")
        {
            if (!Date(raw) || row.DateMeaning != "purchase_business_date" || raw != row.EffectiveDate) return Reject("INVALID_BUSINESS_DATE", "Use the source purchase calendar date; other timestamps cannot substitute.");
        }
        else if (name == "historicalName")
        {
            if (raw.Length > 512 || raw.Any(char.IsControl) || raw.Any(c => char.GetUnicodeCategory(c) == UnicodeCategory.Format) ||
                !WellFormedUnicode(raw)) return Reject("INVALID_HISTORICAL_NAME", "A bounded Unicode source name without control characters is required.");
        }
        else if (name == "historicalUnit")
        {
            if (!Units.Contains(Unit(raw))) return Reject("UNSUPPORTED_UNIT", "Source unit semantics are not supported; no conversion is invented.");
            if (target != null && (string.IsNullOrWhiteSpace(target.LineUnit) || Unit(target.LineUnit) != Unit(raw))) return Reject("UNIT_CORRECTION_REQUIRED", "Existing line Unit is assertion-only; a separate correction contract is required.");
        }
        else
        {
            if (!Number(raw, name == "sellingRate" ? 2 : 3, out var number, name == "totalWeight" ? 99999999999.999m : null)) return Reject("INVALID_DECIMAL", "Use a nonnegative invariant source decimal within range and precision; never round.");
            if (name == "sellingRate" && (row.Currency != "INR" || row.RateBasis != "per_purchase_quantity_unit" ||
                row.SellingCostAlias != null && (!Number(row.SellingCostAlias, 2, out var alias) || alias != number)))
                return Reject("RATE_SEMANTICS_CONFLICT", "INR per purchase quantity unit and consistent selling aliases are required.");
            if (name == "openingStock" && (!Units.Contains(Unit(row.OriginalStockUnit)))) return Reject("MISSING_OPENING_UNIT", "The original stock event unit is required, independent of current catalog units.");
            if (name == "kgPerUnit")
            {
                if (number == 0 || target != null && (target.KgPerUnit == null || target.KgPerUnit != number)) return Reject("GEOMETRY_CORRECTION_REQUIRED", "Positive per-pack source geometry must agree with existing line KgPerUnit; no correction is applied.");
                if (Unit(row.Fields.GetValueOrDefault("historicalUnit")?.Value) is "BOX" or "TIN" or "PCS" or "PC" or "PIECE") return Reject("COUNT_ONLY_GEOMETRY_CONFLICT", "Count-only source units do not imply physical weight.");
            }
            if (name == "totalWeight")
            {
                var kg = row.Fields.GetValueOrDefault("kgPerUnit");
                if (kg is not { State: "KNOWN" } || !Number(kg.Value, 3, out var geometry) || geometry <= 0 ||
                    !Number(row.SourceQuantity, 3, out var quantity) || row.QuantityScope is not ("ordered" or "received") || number != quantity * geometry)
                    return Reject("WEIGHT_RELATIONSHIP_CONFLICT", "Explicit source quantity scope and per-pack geometry must agree with the source total weight.");
            }
            if (name == "normalizedQuantity" && (!Units.Contains(Unit(row.OriginalStockUnit)) || row.QuantityScope is not ("ordered" or "received")))
                return Reject("UNKNOWN_NORMALIZED_SCOPE", "The original target stock unit and ordered/received scope are required; no conversion is inferred.");
        }
        return Result("VALID", "SOURCE_VALUE_VALIDATED", name is "historicalUnit" or "kgPerUnit" ? "Assertion only; existing line value stays unchanged." : "Proposed historical metadata only; all current values stay unchanged.", raw);
    }

    private static bool WellFormedUnicode(string text)
    {
        for (var i = 0; i < text.Length; i++)
            if (char.IsHighSurrogate(text[i])) { if (++i == text.Length || !char.IsLowSurrogate(text[i])) return false; }
            else if (char.IsLowSurrogate(text[i])) return false;
        return true;
    }
}
