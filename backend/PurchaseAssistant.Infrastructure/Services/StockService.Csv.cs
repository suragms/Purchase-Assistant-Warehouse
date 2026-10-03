using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Stock;

namespace PurchaseAssistant.Infrastructure.Services;
public partial class StockService
{
    public static void CsvRange(ref DateTime? start, ref DateTime? end)
    {
        static DateTime Utc(DateTime x) => x.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(x, DateTimeKind.Utc) : x.ToUniversalTime();
        if (start.HasValue) start = Utc(start.Value);
        if (end.HasValue) end = Utc(end.Value);
        if (start > end || (start.HasValue && end.HasValue && (end.Value - start.Value).TotalDays > 3660))
            throw new ArgumentException("Choose a valid period of at most ten years.");
    }
    private static string NativeUnit(string unit) => unit.Trim().ToUpperInvariant() switch
    {
        "KGS" or "KILOGRAM" or "KILOGRAMS" => "KG", "PIECE" or "PIECES" => "PCS",
        "BAGS" or "SACK" or "SACKS" => "BAG", "BOXES" => "BOX", "TINS" => "TIN", var value => value
    };
    public async Task<List<StockCsvRow>> GetCsvRowsAsync(string filter, string? search, DateTime? start, DateTime? end, Guid[]? ids, CancellationToken ct, Guid? categoryId = null, Guid? supplierId = null, string? severity = null)
    {
        CsvRange(ref start, ref end);
        if (filter is not ("all" or "low-stock" or "out-of-stock") || search?.Length > 200 || ids?.Length > 5000 || ids?.Contains(Guid.Empty) == true)
            throw new ArgumentException("Check the stock export filters.");
        var business = _currentUser.BusinessId ?? throw new UnauthorizedAccessException();
        var query = FilterStock(GetBaseQuery().Where(x => x.IsActive).Include(x => x.Type).Include(x => x.LastSupplier).AsNoTracking(), categoryId, supplierId, severity);
        if (ids != null)
        {
            if (ids.Length == 0) throw new ArgumentException("Select at least one item.");
            if (await query.CountAsync(x => ids.Contains(x.Id), ct) != ids.Distinct().Count()) throw new KeyNotFoundException("Item not found.");
            query = query.Where(x => ids.Contains(x.Id));
        }
        if (!string.IsNullOrWhiteSpace(search)) { var term = search.Trim().ToLowerInvariant(); query = query.Where(x => x.Name.ToLower().Contains(term) || x.ItemCode.ToLower().Contains(term) || (x.Barcode != null && x.Barcode.ToLower().Contains(term))); }
        if (filter == "low-stock") query = query.Where(x => x.CurrentStock - x.ReservedStock > 0 && x.CurrentStock - x.ReservedStock <= x.ReorderLevel);
        if (filter == "out-of-stock") query = query.Where(x => x.CurrentStock - x.ReservedStock <= 0);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Id).Take(5001).ToListAsync(ct);
        if (items.Count > 5000) throw new ArgumentException("Stock CSV is limited to 5,000 items.");
        var itemIds = items.Select(x => x.Id).ToArray();
        var purchases = ReportService.ReportingPurchases(_context, business);
        if (start.HasValue) purchases = purchases.Where(x => x.CreatedAt >= start.Value);
        if (end.HasValue) purchases = purchases.Where(x => x.CreatedAt <= end.Value);
        var receipts = await _context.PurchaseItems.AsNoTracking().Where(x => x.BusinessId == business && itemIds.Contains(x.CatalogItemId)
            && purchases.Select(p => p.Id).Contains(x.PurchaseOrderId) && x.ReceivedQuantity > 0)
            .GroupBy(x => new { x.CatalogItemId, x.Unit }).Select(g => new { g.Key.CatalogItemId, g.Key.Unit, Quantity = g.Sum(x => x.ReceivedQuantity) }).ToListAsync(ct);
        var receiptMap = receipts.ToLookup(x => x.CatalogItemId);
        var movements = await _context.StockMovements.AsNoTracking().Where(x => x.BusinessId == business && itemIds.Contains(x.CatalogItemId))
            .GroupBy(x => x.CatalogItemId).Select(g => new { Id = g.Key, At = g.Max(x => x.CreatedAt) }).ToDictionaryAsync(x => x.Id, x => x.At, ct);
        static string Status(decimal current, decimal reorder) => current <= 0 ? "out" : reorder > 0 && current <= reorder / 2 ? "critical"
            : (reorder > 0 && current <= reorder) || (reorder <= 0 && current < 1) ? "low" : "healthy";
        return items.Select(x => new StockCsvRow(x.Id, x.Name, x.Category?.BusinessId == business ? x.Category.Name : "",
            x.Type?.BusinessId == business ? x.Type.Name : null, x.DefaultUnit, x.CurrentStock, x.PhysicalStock, x.ReorderLevel,
            receiptMap[x.Id].Any(r => NativeUnit(r.Unit) != NativeUnit(x.DefaultUnit)) ? null : receiptMap[x.Id].Sum(r => r.Quantity),
            Status(x.CurrentStock, x.ReorderLevel), x.LastSupplier?.BusinessId == business ? x.LastSupplier.Name : null,
            movements.TryGetValue(x.Id, out var at) ? DateTime.SpecifyKind(at, DateTimeKind.Utc) : null)).ToList();
    }
}
