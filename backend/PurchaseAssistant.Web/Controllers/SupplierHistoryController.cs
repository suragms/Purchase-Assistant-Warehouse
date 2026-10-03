using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Web.Controllers;

public record SupplierPurchaseRow(Guid Id, string OrderNumber, DateTime CreatedAt, string Status, [property: FinancialField] decimal GrandTotal, [property: FinancialField] decimal PaidAmount);
public record SupplierHistoryResult(List<SupplierPurchaseRow> Items, int TotalCount, [property: FinancialField] decimal TotalSpend, [property: FinancialField] decimal TotalPaid);
[ApiController, Route("api/v1/catalog/suppliers/{id:guid}"), Authorize(Policy = "RequireSupplierView"), Authorize(Policy = "RequirePurchaseView")]
public class SupplierHistoryController(AppDbContext db, ICurrentUserService user) : ControllerBase
{
    [HttpGet("history")]
    public async Task<IActionResult> History(Guid id, DateTime? from = null, DateTime? to = null, int page = 1, CancellationToken ct = default)
    {
        var end = DateTime.SpecifyKind(to ?? DateTime.UtcNow, DateTimeKind.Utc); var start = DateTime.SpecifyKind(from ?? end.AddDays(-365), DateTimeKind.Utc);
        if (end < start || (end - start).TotalDays > 366 || page is < 1 or > 10000) return BadRequest(new { message = "Invalid date range or page." });
        if (!await db.Suppliers.AnyAsync(x => x.Id == id && x.BusinessId == user.BusinessId, ct)) return NotFound();
        var query = db.Purchases.AsNoTracking().Where(x => x.BusinessId == user.BusinessId && x.SupplierId == id && x.CreatedAt >= start && x.CreatedAt <= end && x.Status != PurchaseStatus.Cancelled && x.Status != PurchaseStatus.Draft);
        var count = await query.CountAsync(ct); var spend = await query.SumAsync(x => x.GrandTotal, ct); var paid = await query.SumAsync(x => x.PaidAmount, ct);
        var rows = await query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((page - 1) * 50).Take(50)
            .Select(x => new SupplierPurchaseRow(x.Id, x.OrderNumber, x.CreatedAt, x.Status.ToString(), x.GrandTotal, x.PaidAmount)).ToListAsync(ct);
        return Ok(new SupplierHistoryResult(rows, count, spend, paid));
    }
    [HttpGet("items/{itemId:guid}/price-history"), Authorize(Roles = "Owner,SuperAdmin")]
    public async Task<IActionResult> Prices(Guid id, Guid itemId, string unit = "PCS", CancellationToken ct = default)
    {
        if (unit.Length > 16) return BadRequest();
        if (!await db.Suppliers.AnyAsync(x => x.Id == id && x.BusinessId == user.BusinessId, ct) || !await db.CatalogItems.AnyAsync(x => x.Id == itemId && x.BusinessId == user.BusinessId, ct)) return NotFound();
        // Derive legacy history as well as new purchases from authoritative confirmed lines; never imply a current quote.
        var rows = await db.PurchaseItems.AsNoTracking().Where(x => x.BusinessId == user.BusinessId && x.CatalogItemId == itemId && x.Unit == unit
            && x.PurchaseOrder.SupplierId == id && x.PurchaseOrder.Status != PurchaseStatus.Draft && x.PurchaseOrder.Status != PurchaseStatus.Cancelled)
            .OrderByDescending(x => x.PurchaseOrder.ConfirmedAt).ThenByDescending(x => x.CreatedAt).Take(20)
            .Select(x => new { purchaseId = x.PurchaseOrderId, date = x.PurchaseOrder.ConfirmedAt, x.Unit, x.UnitPrice, x.DiscountPercent, x.TaxPercent, x.KgPerUnit, x.LandingCostPerKg }).ToListAsync(ct);
        return Ok(rows);
    }
}
