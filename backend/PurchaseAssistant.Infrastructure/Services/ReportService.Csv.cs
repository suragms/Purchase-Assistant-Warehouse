using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Reports;

namespace PurchaseAssistant.Infrastructure.Services;
public partial class ReportService
{
    public async Task<List<PurchaseCsvLine>> GetCsvPurchaseLinesAsync(Guid businessId, DateTime? start, DateTime? end, Guid? supplierId, CancellationToken ct)
    {
        StockService.CsvRange(ref start, ref end);
        var purchases = ReportingPurchases(_context, businessId).Where(x => x.Supplier.BusinessId == businessId);
        if (supplierId.HasValue) purchases = purchases.Where(x => x.SupplierId == supplierId);
        if (start.HasValue) purchases = purchases.Where(x => x.CreatedAt >= start.Value);
        if (end.HasValue) purchases = purchases.Where(x => x.CreatedAt <= end.Value);
        if (await purchases.Take(2001).CountAsync(ct) > 2000) throw new ArgumentException("CSV is limited to 2,000 purchases. Choose a shorter range.");
        var rows = await _context.PurchaseItems.AsNoTracking().Where(x => x.BusinessId == businessId && x.CatalogItem.BusinessId == businessId
            && purchases.Select(p => p.Id).Contains(x.PurchaseOrderId)).OrderBy(x => x.PurchaseOrder.CreatedAt).ThenBy(x => x.PurchaseOrderId).ThenBy(x => x.Id)
            .Select(x => new PurchaseCsvLine(x.PurchaseOrderId, x.PurchaseOrder.OrderNumber, x.PurchaseOrder.SupplierId, x.PurchaseOrder.Supplier.Name,
                x.CatalogItemId, x.CatalogItem.Name, x.OrderedQuantity, x.Unit, x.UnitPrice, x.KgPerUnit, x.LandingCostPerKg, x.LineTotal))
            .Take(50001).ToListAsync(ct);
        if (rows.Count > 50000) throw new ArgumentException("CSV is limited to 50,000 lines. Choose a shorter range.");
        return rows;
    }
    private static (string Kind, decimal Qty, decimal? Kg)? Pack(PurchaseCsvLine line)
    {
        var unit = line.Unit.Trim().ToUpperInvariant();
        if (unit.Contains("BAG") || unit.Contains("SACK")) return ("bag", line.Quantity, line.KgPerUnit > 0 ? line.Quantity * line.KgPerUnit : null);
        if (unit.Contains("BOX")) return ("box", line.Quantity, 0m);
        if (unit.Contains("TIN")) return ("tin", line.Quantity, 0m);
        // Explicit reference fallback only: KG line + item name containing a positive pack size <=500 KG.
        if (unit is "KG" or "KGS" or "KILOGRAM" or "KILOGRAMS")
        {
            var match = Regex.Match(line.Item.ToUpperInvariant(), @"(\d{1,3}(?:\.\d{1,2})?)\s*KG\b", RegexOptions.CultureInvariant);
            if (match.Success && decimal.TryParse(match.Groups[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var size)
                && size > 0 && size <= 500 && line.Quantity > 0) return ("bag", line.Quantity / size, line.Quantity);
        }
        return null;
    }
    public async Task<(List<SupplierCsvRow> Suppliers, List<ItemCsvRow> Items)> GetCsvPackReportsAsync(Guid businessId, DateTime? start, DateTime? end, CancellationToken ct)
    {
        var lines = await GetCsvPurchaseLinesAsync(businessId, start, end, null, ct);
        var packed = lines.Select(line => new { Line = line, Pack = Pack(line) }).ToList();
        static decimal? Kg(IEnumerable<decimal?> values) { var list = values.ToList(); return list.Any(x => !x.HasValue) ? null : list.Sum(x => x!.Value); }
        var suppliers = packed.GroupBy(x => x.Line.SupplierId).Select(g => new SupplierCsvRow(
            string.IsNullOrWhiteSpace(g.First().Line.Supplier) ? "-" : g.First().Line.Supplier,
            g.Where(x => x.Pack?.Kind == "bag").Sum(x => x.Pack!.Value.Qty), Kg(g.Where(x => x.Pack?.Kind == "bag").Select(x => x.Pack!.Value.Kg)),
            g.Where(x => x.Pack != null).Sum(x => x.Line.LineTotal))).OrderBy(x => x.Supplier, StringComparer.Ordinal).ToList();
        var items = packed.Where(x => x.Pack != null).GroupBy(x => x.Line.ItemId).Select(g => new ItemCsvRow(g.First().Line.Item,
            Kg(g.Select(x => x.Pack!.Value.Kg)), g.Where(x => x.Pack!.Value.Kind == "bag").Sum(x => x.Pack!.Value.Qty),
            g.Where(x => x.Pack!.Value.Kind == "box").Sum(x => x.Pack!.Value.Qty), g.Where(x => x.Pack!.Value.Kind == "tin").Sum(x => x.Pack!.Value.Qty),
            g.Sum(x => x.Line.LineTotal), g.Select(x => x.Line.PurchaseId).Distinct().Count())).OrderBy(x => x.Item, StringComparer.Ordinal).ToList();
        return (suppliers, items);
    }
}
