using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Web.Controllers;

[ApiController, Route("api/v1/audit"), Authorize(Policy = "RequireSelectedBusiness"), Authorize(Roles = "Owner,SuperAdmin")]
public class AuditController(AppDbContext db, ICurrentUserService user) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(DateTime? from = null, DateTime? to = null, Guid? actor = null, string? action = null, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var end = to ?? DateTime.UtcNow; var start = from ?? end.AddDays(-30);
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc); end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
        if (end < start || (end - start).TotalDays > 366 || page is < 1 or > 10000 || pageSize is < 1 or > 100 || action?.Length > 100) return BadRequest(new { message = "Use a valid date range up to 366 days and page size up to 100." });
        var query = db.SecurityAuditLogs.AsNoTracking().Where(x => x.BusinessId == user.BusinessId && x.CreatedAt >= start && x.CreatedAt <= end);
        if (actor.HasValue) query = query.Where(x => x.UserId == actor);
        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.EventType.Contains(action));
        var count = await query.CountAsync(ct);
        var rows = await query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.UserId, x.EventType, x.Description, x.CreatedAt, x.MetadataJson }).ToListAsync(ct);
        Response.Headers.CacheControl = "private, no-store";
        return Ok(new { items = rows, totalCount = count, page, pageSize });
    }
}
