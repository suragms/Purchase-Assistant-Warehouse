using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
namespace PurchaseAssistant.Web.Controllers;
[ApiController, Route("api/v1/stock/filter-options"), Authorize(Policy = "RequireStockView")]
public class StockFiltersController(AppDbContext db, ICurrentUserService user) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct)
    {
        var categories = await db.Categories.AsNoTracking().OrderBy(x => x.Name).Take(1001).Select(x => new { x.Id, x.Name }).ToListAsync(ct);
        var canSuppliers = user.Role is "Owner" or "SuperAdmin" || user.HasPermission("supplier.view");
        var suppliers = await db.Suppliers.AsNoTracking().Where(x => canSuppliers && x.IsActive).OrderBy(x => x.Name).Take(1001).Select(x => new { x.Id, x.Name }).ToListAsync(ct);
        if (categories.Count > 1000 || suppliers.Count > 1000) return BadRequest(new { message = "Filter lists exceed 1,000 records. Use item search instead." });
        return Ok(new { categories, suppliers });
    }
}
