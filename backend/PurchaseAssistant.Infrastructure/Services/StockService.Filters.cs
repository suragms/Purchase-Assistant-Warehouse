using PurchaseAssistant.Domain.Entities;
namespace PurchaseAssistant.Infrastructure.Services;
public partial class StockService
{
    private IQueryable<CatalogItem> FilterStock(IQueryable<CatalogItem> query, Guid? category, Guid? supplier, string? severity)
    {
        if (severity is not (null or "" or "critical" or "low" or "out" or "healthy")) throw new ArgumentException("Invalid stock severity.");
        if (category.HasValue) query = query.Where(x => x.CategoryId == category);
        if (supplier.HasValue) {
            if (_currentUser.Role is not ("Owner" or "SuperAdmin") && !_currentUser.HasPermission("supplier.view")) throw new UnauthorizedAccessException();
            query = query.Where(x => x.LastSupplierId == supplier || _context.SupplierItems.Any(s => s.CatalogItemId == x.Id && s.SupplierId == supplier && s.BusinessId == x.BusinessId));
        }
        return severity switch {
            "out" => query.Where(x => x.CurrentStock - x.ReservedStock <= 0),
            "critical" => query.Where(x => x.CurrentStock - x.ReservedStock > 0 && x.CurrentStock - x.ReservedStock <= x.ReorderLevel / 2),
            "low" => query.Where(x => x.CurrentStock - x.ReservedStock > x.ReorderLevel / 2 && x.CurrentStock - x.ReservedStock > 0 && x.CurrentStock - x.ReservedStock <= x.ReorderLevel),
            "healthy" => query.Where(x => x.CurrentStock - x.ReservedStock > x.ReorderLevel && x.CurrentStock - x.ReservedStock > 0), _ => query
        };
    }
}
