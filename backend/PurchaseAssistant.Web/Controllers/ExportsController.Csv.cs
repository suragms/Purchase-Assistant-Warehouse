using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Infrastructure.Services;
using System.Globalization;

namespace PurchaseAssistant.Web.Controllers;
public partial class ExportsController
{
    private static ExportFileBuilder.CsvNumber Fixed(decimal value, int places) => new(decimal.Round(value, places, MidpointRounding.AwayFromZero), "F" + places);
    private static ExportFileBuilder.CsvNumber StockDisplay(decimal value, string unit)
    {
        var discrete = unit.Trim().ToLowerInvariant() is "bag" or "bags" or "sack" or "sacks" or "box" or "boxes" or "tin" or "tins" or "pcs" or "piece" or "pieces" or "pkt" or "packet";
        return discrete ? new(decimal.Round(value, 0, MidpointRounding.AwayFromZero), "#,0") : ReportDisplay(value);
    }
    private static ExportFileBuilder.CsvNumber ReportDisplay(decimal value) => Math.Abs(value - decimal.Round(value, 0, MidpointRounding.AwayFromZero)) < 0.001m
        ? new(decimal.Round(value, 0, MidpointRounding.AwayFromZero), "#,0") : new(decimal.Round(value, 2, MidpointRounding.AwayFromZero), "0.0#");
    [HttpGet("stock.csv"), Authorize(Policy = "RequireStockView")]
    public async Task<IActionResult> StockCsv(string filter = "all", string? search = null, DateTime? start = null, DateTime? end = null, [FromQuery] Guid[]? ids = null, CancellationToken ct = default)
    {
        var rows = await stockService.GetCsvRowsAsync(filter, search, start, end, ids, ct);
        return await Download(ExportFileBuilder.Csv(["Item", "Category", "Subcategory", "Unit", "Current Stock", "Opening Stock", "Purchased", "Reorder Level", "Last Updated"],
            rows.Select(x => new object?[] { x.Name, x.Category, x.Subcategory, x.Unit, x.Current, null, x.Purchased, x.Reorder,
                x.LastMovement?.ToString("O", CultureInfo.InvariantCulture) })), "text/csv; charset=utf-8", "harisree_stock_export.csv");
    }
    [HttpGet("low-stock.csv"), Authorize(Policy = "RequireStockView")]
    public async Task<IActionResult> LowStockCsv(string? search = null, DateTime? start = null, DateTime? end = null, [FromQuery] Guid[]? ids = null, CancellationToken ct = default)
    {
        var rows = await stockService.GetCsvRowsAsync("low-stock", search, start, end, ids, ct);
        return await Download(ExportFileBuilder.Csv(["name", "subcategory", "unit", "system_stock", "physical_stock", "reorder", "purchased", "status", "supplier"],
            rows.Select(x => new object?[] { x.Name, x.Subcategory ?? x.Category, x.Unit, StockDisplay(x.Current, x.Unit), StockDisplay(x.Physical, x.Unit),
                x.Reorder > 0 ? StockDisplay(x.Reorder, x.Unit) : null, x.Purchased > 0 ? StockDisplay(x.Purchased.Value, x.Unit) : null, x.Status, x.Supplier })),
            "text/csv; charset=utf-8", "harisree_low_stock.csv");
    }
    [HttpGet("suppliers/{supplierId:guid}/purchases.csv"), Authorize(Roles = "Owner,SuperAdmin"), Authorize(Policy = "RequireSupplierView"), Authorize(Policy = "RequirePurchaseView")]
    public async Task<IActionResult> SupplierCsv(Guid supplierId, DateTime? start, DateTime? end, CancellationToken ct)
    {
        StockService.CsvRange(ref start, ref end);
        if (!await db.Suppliers.AsNoTracking().AnyAsync(x => x.Id == supplierId && x.BusinessId == Business && x.IsActive, ct)) return NotFound(new { message = "Supplier not found." });
        var rows = await reportService.GetCsvPurchaseLinesAsync(Business, start, end, supplierId, ct);
        return await Download(ExportFileBuilder.Csv(["date", "pur_id", "item", "qty", "unit", "landing_per_unit", "selling", "total_line"],
            rows.Select(x => new object?[] { null, x.OrderNumber, x.Item, x.Quantity, x.Unit,
                Fixed(x.KgPerUnit > 0 && x.PerKgRate.HasValue ? x.PerKgRate.Value : x.UnitPrice, 2), null, Fixed(x.LineTotal, 2) })),
            "text/csv; charset=utf-8", $"harisree_supplier_{supplierId:N}.csv");
    }
    private static string ReportComment(string name, DateTime? start, DateTime? end) => $"# Harisree Reports — {name} — {start?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "all"} to {end?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "all"}";
    [HttpGet("reports/suppliers.csv"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> SupplierReportCsv(DateTime? start, DateTime? end, CancellationToken ct)
    {
        StockService.CsvRange(ref start, ref end);
        var report = await reportService.GetCsvPackReportsAsync(Business, start, end, ct);
        return await Download(ExportFileBuilder.Csv(["supplier", "bag_qty", "bag_kg", "amount_inr"],
            report.Suppliers.Select(x => new object?[] { x.Supplier, ReportDisplay(x.Bags), x.BagKg.HasValue ? ReportDisplay(x.BagKg.Value) : null, Fixed(x.Amount, 0) }), ReportComment("purchases", start, end)),
            "text/csv; charset=utf-8", "harisree_report_suppliers.csv");
    }
    [HttpGet("reports/items.csv"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> ItemReportCsv(DateTime? start, DateTime? end, CancellationToken ct)
    {
        StockService.CsvRange(ref start, ref end);
        var report = await reportService.GetCsvPackReportsAsync(Business, start, end, ct);
        return await Download(ExportFileBuilder.Csv(["name", "kg", "bags", "boxes", "tins", "amount_inr", "purchase_count"],
            report.Items.Select(x => new object?[] { x.Item, x.Kg.HasValue ? ReportDisplay(x.Kg.Value) : null, ReportDisplay(x.Bags), ReportDisplay(x.Boxes), ReportDisplay(x.Tins), Fixed(x.Amount, 0), x.Purchases }), ReportComment("items", start, end)),
            "text/csv; charset=utf-8", "harisree_report_items.csv");
    }
}
