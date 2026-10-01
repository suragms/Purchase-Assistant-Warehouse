using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using System.IO.Compression;
using System.Text.Json;
using System.Globalization;
namespace PurchaseAssistant.Web.Controllers;
[ApiController, Route("api/v1/exports"), Authorize(Policy = "RequireReportsView"), Authorize(Roles = "Owner,Admin,Manager,SuperAdmin")]
public class ExportsController(AppDbContext db, ICurrentUserService user) : ControllerBase
{
    private bool Money => user.Role is "Owner" or "SuperAdmin";
    private static string Amount(decimal v) => v.ToString("0.00##", CultureInfo.InvariantCulture);
    private IQueryable<PurchaseAssistant.Domain.Entities.PurchaseOrder> Orders(DateTime? start, DateTime? end)
    {
        if (start > end) throw new ArgumentException("Invalid date range.");
        var q = db.Purchases.AsNoTracking().Where(x => x.BusinessId == user.BusinessId && x.Status != PurchaseStatus.Draft && x.Status != PurchaseStatus.Cancelled);
        if (start.HasValue) { var from = DateTime.SpecifyKind(start.Value, DateTimeKind.Utc); q = q.Where(x => x.CreatedAt >= from); }
        if (end.HasValue) { var to = DateTime.SpecifyKind(end.Value, DateTimeKind.Utc); q = q.Where(x => x.CreatedAt <= to); }
        return q;
    }
    private async Task<byte[]> Stock(CancellationToken ct)
    {
        var rows = await db.CatalogItems.AsNoTracking().Where(x => x.BusinessId == user.BusinessId && x.IsActive).OrderBy(x => x.Name)
            .Select(x => new { x.ItemCode, x.Name, Category = x.Category.Name, x.DefaultUnit, x.CurrentStock, x.PhysicalStock, x.ReservedStock, x.ReorderLevel }).Take(25001).ToListAsync(ct);
        if (rows.Count > 25000) throw new ArgumentException("Stock export is limited to 25,000 items.");
        return ExportFileBuilder.Spreadsheet(["Code", "Item", "Category", "Unit", "System stock", "Physical stock", "Reserved", "Available", "Reorder level"],
            rows.Select(x => new object?[] { x.ItemCode, x.Name, x.Category, x.DefaultUnit, x.CurrentStock, x.PhysicalStock, x.ReservedStock, x.CurrentStock - x.ReservedStock, x.ReorderLevel }));
    }
    private FileStreamResult Download(byte[] bytes, string mime, string name)
    {
        Response.Headers.CacheControl = "private, no-store"; Response.Headers.XContentTypeOptions = "nosniff";
        return File(new MemoryStream(bytes, false), mime, name);
    }
    [HttpGet("stock.xlsx")]
    public async Task<IActionResult> StockFile(CancellationToken ct) => Download(await Stock(ct), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "stock-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".xlsx");
    private IEnumerable<string> PurchaseRows(PurchaseAssistant.Domain.Entities.PurchaseOrder p)
    {
        yield return p.OrderNumber + " | " + p.CreatedAt.ToString("yyyy-MM-dd") + " | " + p.Supplier.Name + " | " + p.Status;
        foreach (var i in p.Items) yield return i.CatalogItem.Name + " | Qty " + Amount(i.OrderedQuantity) + " | Received " + Amount(i.ReceivedQuantity) + (Money ? " | Unit INR " + Amount(i.UnitPrice) + " | Total INR " + Amount(i.LineTotal) : "");
        if (Money) yield return "Total INR " + Amount(p.GrandTotal) + " | Paid INR " + Amount(p.PaidAmount) + " | Outstanding INR " + Amount(p.GrandTotal - p.PaidAmount);
        yield return "";
    }
    [HttpGet("purchases.pdf")]
    public async Task<IActionResult> Purchases(DateTime? start, DateTime? end, CancellationToken ct)
    {
        var rows = await Orders(start, end).Include(x => x.Supplier).Include(x => x.Items).ThenInclude(x => x.CatalogItem).OrderByDescending(x => x.CreatedAt).Take(401).ToListAsync(ct);
        if (rows.Count > 400) return StatusCode(413, new { message = "Choose a shorter range (maximum 400 purchases)." });
        return Download(ExportFileBuilder.Pdf("Purchase report", rows.SelectMany(PurchaseRows)), "application/pdf", "purchases.pdf");
    }
    [HttpGet("backup.json")]
    public async Task<IActionResult> JsonBackup(DateTime? start, DateTime? end, CancellationToken ct)
    {
        var orders = await Orders(start, end).Include(x => x.Supplier).Include(x => x.Items).ThenInclude(x => x.CatalogItem).Take(5001).ToListAsync(ct);
        var stock = await db.CatalogItems.AsNoTracking().Where(x => x.BusinessId == user.BusinessId).Select(x => new { x.Id, x.Name, x.ItemCode, x.DefaultUnit, x.CurrentStock, x.PhysicalStock, x.ReservedStock, x.ReorderLevel }).Take(25001).ToListAsync(ct);
        if (orders.Count > 5000 || stock.Count > 25000) return StatusCode(413, new { message = "Export is too large. Choose a shorter purchase range." });
        var purchases = orders.Select(p => { var row = new Dictionary<string, object?> { ["id"] = p.Id, ["orderNumber"] = p.OrderNumber, ["date"] = p.CreatedAt, ["supplier"] = p.Supplier.Name, ["status"] = p.Status.ToString(),
            ["items"] = p.Items.Select(i => { var line = new Dictionary<string, object?> { ["catalogItemId"] = i.CatalogItemId, ["name"] = i.CatalogItem.Name, ["orderedQuantity"] = i.OrderedQuantity, ["receivedQuantity"] = i.ReceivedQuantity }; if (Money) { line["unitPrice"] = i.UnitPrice; line["lineTotal"] = i.LineTotal; } return line; }).ToArray() };
            if (Money) { row["grandTotal"] = p.GrandTotal; row["paidAmount"] = p.PaidAmount; } return row; }).ToArray();
        var suppliers = await db.Suppliers.AsNoTracking().Where(x => x.BusinessId == user.BusinessId).OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.Phone, x.Address }).Take(2001).ToListAsync(ct);
        var movements = await db.StockMovements.AsNoTracking().Where(x => x.BusinessId == user.BusinessId).OrderByDescending(x => x.CreatedAt).Select(x => new { x.Id, x.CatalogItemId, x.MovementType, x.QuantityDelta, x.QuantityBefore, x.QuantityAfter, x.CreatedAt }).Take(500).ToListAsync(ct);
        if (suppliers.Count > 2000) return StatusCode(413, new { message = "Supplier export limit exceeded." });
        var backup = new { schemaVersion = 1, businessId = user.BusinessId, exportedAt = DateTime.UtcNow, stock, suppliers, purchases, stockMovements = movements };
        return Download(JsonSerializer.SerializeToUtf8Bytes(backup), "application/json", "business-backup.json");
    }
    [HttpGet("backup.zip")]
    public async Task<IActionResult> ZipBackup(DateTime? start, DateTime? end, CancellationToken ct)
    {
        var rows = await Orders(start, end).Include(x => x.Supplier).Include(x => x.Items).ThenInclude(x => x.CatalogItem).OrderByDescending(x => x.CreatedAt).Take(401).ToListAsync(ct);
        if (rows.Count > 400) return StatusCode(413, new { message = "Choose a shorter range (maximum 400 purchases)." });
        if (rows.Count == 0) return NotFound(new { message = "No purchases in this range." });
        using var output = new MemoryStream();
        using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
        {
            async Task Add(string path, byte[] bytes) { using var stream = zip.CreateEntry(path).Open(); await stream.WriteAsync(bytes, ct); }
            await Add("purchases_summary.pdf", ExportFileBuilder.Pdf("Purchase summary", rows.SelectMany(PurchaseRows)));
            await Add("stock/stock.xlsx", await Stock(ct));
            foreach (var p in rows) await Add($"orders/{p.Id:N}.pdf", ExportFileBuilder.Pdf(p.OrderNumber, PurchaseRows(p)));
            foreach (var group in rows.GroupBy(x => x.SupplierId)) await Add($"ledgers/{group.Key:N}.pdf", ExportFileBuilder.Pdf("Supplier ledger", group.SelectMany(PurchaseRows)));
        }
        return Download(output.ToArray(), "application/zip", "business-backup.zip");
    }
    [HttpPost("restore/dry-run"), Authorize(Roles = "Owner,SuperAdmin"), RequestSizeLimit(10485760)]
    public IActionResult DryRun([FromBody] JsonElement payload)
    {
        var errors = new List<string>();
        if (payload.ValueKind != JsonValueKind.Object) return BadRequest(new { message = "A backup object is required." });
        if (!payload.TryGetProperty("businessId", out var business) || !Guid.TryParse(business.GetString(), out var id) || id != user.BusinessId) errors.Add("Backup belongs to another business or has no business ID.");
        if (!payload.TryGetProperty("schemaVersion", out var version) || !version.TryGetInt32(out var v) || v != 1) errors.Add("Unsupported backup version.");
        var counts = new Dictionary<string, int>();
        foreach (var key in new[] { "stock", "suppliers", "purchases", "stockMovements" }) { if (!payload.TryGetProperty(key, out var rows) || rows.ValueKind != JsonValueKind.Array) errors.Add($"Missing {key} array."); else counts[key] = rows.GetArrayLength(); }
        return Ok(new { valid = errors.Count == 0, errors, rowCounts = counts, writesPerformed = false, restoreEnabled = false });
    }
    [HttpPost("restore/commit"), Authorize(Roles = "Owner,SuperAdmin")]
    public IActionResult Restore() => StatusCode(501, new { message = "Restore requires an approved production-copy rehearsal and is not enabled." });
}
