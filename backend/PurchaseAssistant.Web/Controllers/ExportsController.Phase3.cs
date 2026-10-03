using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Infrastructure.Services;
namespace PurchaseAssistant.Web.Controllers;
public partial class ExportsController
{
    [HttpGet("audit.csv"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> AuditCsv(DateTime? from, DateTime? to, Guid? actor, string? action, CancellationToken ct)
    {
        var end = Utc(to ?? DateTime.UtcNow); var start = Utc(from ?? end.AddDays(-30));
        if (end < start || (end - start).TotalDays > 366 || action?.Length > 100) return BadRequest(new { message = "Choose a valid range up to 366 days." });
        var q = db.SecurityAuditLogs.AsNoTracking().Where(x => x.BusinessId == Business && x.CreatedAt >= start && x.CreatedAt <= end);
        if (actor.HasValue) q = q.Where(x => x.UserId == actor);
        if (!string.IsNullOrWhiteSpace(action)) q = q.Where(x => x.EventType.Contains(action));
        var rows = await q.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Take(5001).ToListAsync(ct);
        if (rows.Count > 5000) return StatusCode(413, new { message = "Narrow the audit filters to at most 5,000 records." });
        return await Download(ExportFileBuilder.Csv(["Timestamp UTC", "User", "Action", "Entity", "Changes"], rows.Select(x => new object?[] { x.CreatedAt.ToString("O"), x.UserId, x.EventType, x.Description, x.MetadataJson })), "text/csv; charset=utf-8", "audit.csv");
    }
    [HttpGet("movements.csv"), Authorize(Policy = "RequireStockView")]
    public async Task<IActionResult> MovementCsv(DateTime? start, DateTime? end, Guid? itemId, Guid? actor, CancellationToken ct)
    {
        StockService.CsvRange(ref start, ref end);
        var q = db.StockMovements.AsNoTracking().Where(x => x.BusinessId == Business);
        if (start.HasValue) q = q.Where(x => x.CreatedAt >= start); if (end.HasValue) q = q.Where(x => x.CreatedAt <= end);
        if (itemId.HasValue) q = q.Where(x => x.CatalogItemId == itemId); if (actor.HasValue) q = q.Where(x => x.CreatedById == actor);
        var rows = await q.Include(x => x.CatalogItem).OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Take(5001).ToListAsync(ct);
        if (rows.Count > 5000) return StatusCode(413, new { message = "Narrow the movement filters to at most 5,000 records." });
        return await Download(ExportFileBuilder.Csv(["Timestamp UTC", "Item", "Type", "Change", "Before", "After", "User", "Reason"], rows.Select(x => new object?[] { x.CreatedAt.ToString("O"), x.CatalogItem.Name, x.MovementType, x.QuantityDelta, x.QuantityBefore, x.QuantityAfter, x.CreatedById, x.Reason })), "text/csv; charset=utf-8", "movements.csv");
    }
    [HttpGet("ml/{id:guid}.csv"), Authorize(Policy = "RequireStockView")]
    public async Task<IActionResult> ForecastCsv(Guid id, [FromServices] MlService ml, int horizon = 7, CancellationToken ct = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct); timeout.CancelAfter(TimeSpan.FromSeconds(15));
        var result = await ml.AnalyzeAsync(id, horizon, timeout.Token);
        if (result.Status != "ready") return Conflict(new { message = result.Message });
        return await Download(ExportFileBuilder.Csv(["Item", "Unit", "Date", "Forecast", "Lower scenario", "Upper scenario", "Model version", "Trained UTC"], result.Forecast.Select(x => new object?[] { result.ItemName, result.Unit, x.Date.ToString("yyyy-MM-dd"), x.Quantity, x.Lower, x.Upper, result.ModelVersion, result.TrainedAt?.ToString("O") })), "text/csv; charset=utf-8", "forecast.csv");
    }
}
