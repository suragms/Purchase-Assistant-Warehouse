using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Services;

namespace PurchaseAssistant.Infrastructure.Services;

// Fabricated examples only. No catalog, purchase or supplier query is made by the preview API.
public static class HistoricalPreviewFixtures
{
    public static readonly string[] Suites = ["valid", "missing", "conflicts", "matching", "provenance", "mixed"];
    public static (HistoricalPreviewInput Input, IReadOnlyList<HistoricalTarget> Targets) Create(string suite, Guid business, Guid actor)
    {
        if (!Suites.Contains(suite)) throw new ArgumentException("Select a supported synthetic fixture.");
        var item = Guid.Parse("11111111-1111-4111-8111-111111111111");
        var other = Guid.Parse("22222222-2222-4222-8222-222222222222");
        var purchase = Guid.Parse("33333333-3333-4333-8333-333333333333");
        var line = Guid.Parse("44444444-4444-4444-8444-444444444444");
        var supplier = Guid.Parse("55555555-5555-4555-8555-555555555555");
        var foreign = Guid.Parse("66666666-6666-4666-8666-666666666666");
        var targets = new List<HistoricalTarget> {
            new(business, item, "SYN-1", "SYN-BAR-1", purchase, line, supplier, "Synthetic current item", "BAG", "BAG", 8.125m, 7.5m, 2.5m),
            new(business, other, "SYN-DUP", "SYN-BAR-2", purchase, Guid.Parse("77777777-7777-4777-8777-777777777777"), supplier, "Synthetic second item", "BOX", "BOX", 3m, 3m, null),
            new(business, Guid.Parse("88888888-8888-4888-8888-888888888888"), "SYN-DUP", "SYN-BAR-3", purchase, Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), supplier, "Synthetic duplicate code", "BOX", "BOX", 2m, 2m, null),
            new(foreign == business ? Guid.Empty : foreign, foreign, "FOREIGN", "FOREIGN", foreign, foreign, foreign, "Foreign synthetic item", "KG", "KG", 99m, 99m, null) };
        var maps = new List<HistoricalSourceMap> { new("synthetic-source", business, "source-item-1", item), new("synthetic-source", business, "source-item-2", other) };
        var rows = new List<(string Suite, HistoricalRow Row)>();
        HistoricalValue Known(string value) => new("KNOWN", value, value);
        HistoricalRow Base(string name) => new(name, business, item, "SYN-1", "SYN-BAR-1", "source-item-1", purchase, line, supplier,
            "2020-01-02", "purchase_business_date", "INR", "per_purchase_quantity_unit", "BAG", "ordered", "1.2",
            new("synthetic-source", "SyntheticFixture", "synthetic-import", name, name, actor, "2026-10-02T00:00:00Z", "2020-01-02T05:30:00+05:30"),
            new Dictionary<string, HistoricalValue> { ["openingStock"] = Known("5.125"), ["businessDate"] = Known("2020-01-02"),
                ["sellingRate"] = Known("12.25"), ["historicalName"] = Known("Synthetic historical item"), ["historicalUnit"] = Known("BAG"),
                ["kgPerUnit"] = Known("2.5"), ["totalWeight"] = Known("3"), ["normalizedQuantity"] = Known("1.2") });
        HistoricalRow WithField(HistoricalRow row, string field, HistoricalValue value) => row with { Fields = row.Fields.ToDictionary(x => x.Key, x => x.Key == field ? value : x.Value) };
        void Add(string group, string name, Func<HistoricalRow, HistoricalRow>? change = null) { var row = Base(name); rows.Add((group, change?.Invoke(row) ?? row)); }
        Add("valid", "fully-valid");
        Add("valid", "known-zero-rate", r => WithField(r, "sellingRate", Known("0")));
        Add("valid", "valid-name"); Add("valid", "valid-unit"); Add("valid", "valid-weight");
        Add("valid", "unicode-name", r => WithField(r, "historicalName", Known("സിന്തറ്റിക് അരി · اختبار · 米 🌾")));
        Add("valid", "decimal-quantity"); Add("valid", "decimal-rate");
        Add("valid", "zero-opening-quantity", r => WithField(r, "openingStock", Known("0")));
        foreach (var (name, field) in new[] { ("missing-opening", "openingStock"), ("missing-date", "businessDate"), ("missing-rate", "sellingRate"),
            ("missing-name", "historicalName"), ("blank-unit", "historicalUnit"), ("missing-weight", "kgPerUnit") })
            Add("missing", name, r => WithField(r, field, new("UNKNOWN")));
        Add("valid", "not-captured-rate", r => WithField(r, "sellingRate", new("NOT_CAPTURED")));
        Add("valid", "not-applicable-weight", r => WithField(WithField(WithField(r with { ItemId = other, ItemCode = "SYN-DUP", Barcode = "SYN-BAR-2", SourceItemIdentifier = "source-item-2", LineId = targets[1].LineId }, "historicalUnit", Known("BOX")), "kgPerUnit", new("NOT_APPLICABLE")), "totalWeight", new("NOT_APPLICABLE")) with { ItemCode = null });
        Add("conflicts", "conflicting-rate-alias", r => r with { SellingCostAlias = "88" });
        Add("conflicts", "conflicting-unit", r => WithField(r, "historicalUnit", Known("PCS")));
        Add("conflicts", "conflicting-geometry", r => WithField(r, "kgPerUnit", Known("3")));
        Add("conflicts", "negative-quantity", r => WithField(r, "openingStock", Known("-1")));
        Add("conflicts", "invalid-date", r => WithField(r, "businessDate", Known("2020-02-30")));
        Add("conflicts", "invalid-decimal", r => WithField(r, "sellingRate", Known("12,25")));
        Add("conflicts", "excess-precision", r => WithField(r, "openingStock", Known("1.2345")));
        Add("conflicts", "unclear-rate-basis", r => r with { RateBasis = "legacy_unknown" });
        Add("conflicts", "unknown-normalized-unit", r => r with { OriginalStockUnit = "" });
        Add("matching", "ambiguous-item-code", r => r with { ItemId = null, ItemCode = "SYN-DUP", Barcode = null });
        Add("matching", "conflicting-barcode", r => r with { Barcode = "SYN-BAR-2" });
        Add("matching", "item-not-found", r => r with { ItemId = Guid.Parse("99999999-9999-4999-8999-999999999999") });
        // Clear alternate keys so a missing ID does not masquerade as an ambiguous code.
        rows[^1] = ("matching", rows[^1].Row with { ItemCode = null, Barcode = null, SourceItemIdentifier = "missing-source-item" });
        Add("matching", "cross-tenant-item", r => r with { ItemId = foreign });
        Add("matching", "cross-tenant-supplier", r => r with { SupplierId = foreign });
        Add("matching", "cross-tenant-business", r => r with { BusinessId = targets[3].BusinessId });
        Add("matching", "invalid-source-identifier", r => r with { SourceItemIdentifier = "../secret" });
        Add("provenance", "unknown-provenance", r => r with { Provenance = r.Provenance with { SourceIdentifier = "unknown-source" } });
        Add("provenance", "missing-source", r => r with { Provenance = r.Provenance with { SourceIdentifier = "" } });
        Add("provenance", "missing-import-identifier", r => r with { Provenance = r.Provenance with { ImportIdentifier = "" } });
        Add("provenance", "missing-row-identifier", r => r with { Provenance = r.Provenance with { RowIdentifier = "" } });
        Add("provenance", "missing-actor", r => r with { Provenance = r.Provenance with { Actor = Guid.Empty } });
        Add("provenance", "conflicting-original-value", r => WithField(r, "historicalName", new("KNOWN", "Changed", "Original")));
        Add("provenance", "correction-without-reason", r => r with { Provenance = r.Provenance with { PriorRevision = "revision-1" } });
        Add("provenance", "duplicate-source-row-a", r => r with { Provenance = r.Provenance with { SourceRowIdentifier = "duplicate-source" } });
        Add("provenance", "duplicate-source-row-b", r => r with { Provenance = r.Provenance with { SourceRowIdentifier = "duplicate-source" } });
        Add("provenance", "duplicate-import-row-a", r => r with { Provenance = r.Provenance with { RowIdentifier = "duplicate-import" } });
        Add("provenance", "duplicate-import-row-b", r => r with { Provenance = r.Provenance with { RowIdentifier = "duplicate-import" } });
        return (new(HistoricalMetadataValidator.SchemaVersion, true, business, rows.Where(x => suite == "mixed" || x.Suite == suite).Select(x => x.Row).ToArray(), maps), targets);
    }
}
