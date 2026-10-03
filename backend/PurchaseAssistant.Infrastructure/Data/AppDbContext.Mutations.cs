using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Domain.Constants;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PurchaseAssistant.Infrastructure.Data;

public partial class AppDbContext
{
    // Explicit allowlist: never serialize a tracked entity, credential, password, token or request body.
    private static readonly HashSet<string> AuditFields = ["Name", "Email", "Status", "IsActive", "Role", "PermissionsJson", "ItemCode", "Barcode", "CategoryId", "TypeId", "DefaultUnit", "KgPerUnit", "ReorderLevel", "CurrentStock", "PhysicalStock", "ReservedStock", "SupplierId", "CatalogItemId", "SupplierItemCode", "IsDefault", "QuantityDelta", "QuantityBefore", "QuantityAfter", "MovementType", "UsedQty", "IsConfirmed", "Date", "StaffId", "TaskType", "Rejected", "NotificationsEnabled", "NotificationKindsJson", "BrandingTitle", "GstNumber", "Address", "Phone", "ContactEmail"];
    private async Task<List<Notification>> PrepareMutationRecordsAsync(CancellationToken ct)
    {
        if (_auditUser?.UserId == null || _auditUser.BusinessId is not Guid business || business == Guid.Empty) return [];
        ChangeTracker.DetectChanges();
        var changes = ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).ToArray();
        foreach (var entry in changes.Where(e => e.Entity is CatalogItem or CatalogVariant or Category or CategoryType or Supplier or Broker or SupplierItem or BrokerSupplier or StockMovement or DailyUsageLog or StaffTask or ChecklistTemplate or ChecklistCompletion or UserSettings or Business or User or Membership)) {
            if (entry.Entity is PurchaseAssistant.Domain.Common.TenantEntity tenant && tenant.BusinessId != business) throw new UnauthorizedAccessException("Mutation outside selected business.");
            var fields = entry.Properties.Where(p => AuditFields.Contains(p.Metadata.Name) && (entry.State != EntityState.Modified || p.IsModified)).ToDictionary(p => p.Metadata.Name,
                p => (object)new { oldValue = entry.State == EntityState.Added ? null : p.OriginalValue, newValue = entry.State == EntityState.Deleted ? null : p.CurrentValue });
            if (fields.Count == 0) continue;
            var id = entry.Property("Id").CurrentValue;
            // Failed SaveChanges may be retried with the same tracker; retain just one pending audit per entity/action.
            var description = $"{entry.Metadata.ClrType.Name}:{id}"; var action = entry.Metadata.ClrType.Name + entry.State;
            if (SecurityAuditLogs.Local.Any(x => Entry(x).State == EntityState.Added && x.Description == description && x.EventType == action)) continue;
            SecurityAuditLogs.Add(new() { BusinessId = business, UserId = _auditUser.UserId, EventType = action, Description = description,
                MetadataJson = JsonSerializer.Serialize(new { entity = entry.Metadata.ClrType.Name, entityId = id, action = entry.State.ToString(), changes = fields }) });
        }
        var events = new List<(NotificationType Type, string Kind, string Permission, string Title, string Message, string Reference, Guid Id)>();
        foreach (var entry in changes) {
            if (entry.Entity is CatalogItem item && item.BusinessId == business && item.IsActive
                && (entry.State == EntityState.Added || entry.Properties.Any(p => p.IsModified && p.Metadata.Name is "CurrentStock" or "ReservedStock" or "ReorderLevel"))) {
                var available = item.CurrentStock - item.ReservedStock;
                if (available <= item.ReorderLevel) events.Add((available <= 0 ? NotificationType.OutOfStock : NotificationType.LowStock, "low_stock", Permissions.StockView,
                    available <= 0 ? "Out of stock" : available <= item.ReorderLevel / 2 ? "Critical stock" : "Low stock", $"{item.Name} has {available:0.####} {item.DefaultUnit} available.", "CatalogItem", item.Id));
            }
            if (entry.Entity is StockMovement movement && entry.State == EntityState.Added) {
                if (movement.MovementType.Contains("Physical", StringComparison.OrdinalIgnoreCase) || movement.MovementType.Contains("Reconcil", StringComparison.OrdinalIgnoreCase))
                    events.Add((NotificationType.StockVariance, "stock_variance", Permissions.StockView, "Stock count updated", "A physical count or reconciliation was recorded. Review item activity.", "CatalogItem", movement.CatalogItemId));
            }
            if (entry.Entity is PurchaseOrder order && entry.State == EntityState.Added)
                events.Add((NotificationType.PurchasePending, "staff_alert", Permissions.PurchaseView, "Purchase created", "A new purchase draft is available for review.", "PurchaseOrder", order.Id));
            if (entry.Entity is Membership membership && entry.State != EntityState.Deleted)
                events.Add((NotificationType.System, "staff_alert", Permissions.UsersView, "Staff access updated", "A business membership or permission assignment changed.", "Membership", membership.Id));
            if (entry.Entity is MlPredictionLog prediction && entry.State == EntityState.Added) {
                var predictedItem = await CatalogItems.AsNoTracking().SingleOrDefaultAsync(x => x.BusinessId == business && x.Id == prediction.CatalogItemId, ct);
                if (predictedItem != null && prediction.PredictedQuantity > predictedItem.CurrentStock - predictedItem.ReservedStock)
                    events.Add((NotificationType.System, "staff_alert", Permissions.StockView, "Forecast stock alert", "Forecast consumption exceeds available stock. Review the prediction and uncertainty before purchasing.", "MlPrediction", prediction.CatalogItemId));
            }
        }
        if (events.Count == 0) return [];
        var members = await Memberships.AsNoTracking().Where(x => x.BusinessId == business && x.Business.IsActive && x.User.Status == UserStatus.Active).ToListAsync(ct);
        var settings = await Set<UserSettings>().AsNoTracking().Where(x => x.BusinessId == business).ToDictionaryAsync(x => x.UserId, ct);
        var unread = (await Notifications.AsNoTracking().Where(x => x.BusinessId == business && !x.IsRead).Select(x => new { x.UserId, x.DedupeKey }).ToListAsync(ct)).Select(x => (x.UserId, x.DedupeKey)).ToHashSet();
        var output = new List<Notification>();
        foreach (var e in events) foreach (var member in members) {
            string[] permissions;
            try { permissions = string.IsNullOrWhiteSpace(member.PermissionsJson) ? Permissions.ForRole(member.Role) : JsonSerializer.Deserialize<string[]>(member.PermissionsJson) ?? []; }
            catch (JsonException) { continue; }
            if (member.Role is not (Role.Owner or Role.SuperAdmin) && !permissions.Contains(e.Permission)) continue;
            if (settings.TryGetValue(member.UserId, out var preference) && (!preference.NotificationsEnabled || !(JsonSerializer.Deserialize<string[]>(preference.NotificationKindsJson) ?? []).Contains(e.Kind))) continue;
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{e.Type}:{e.Reference}:{e.Id}")));
            if (!unread.Add((member.UserId, key)) || Notifications.Local.Any(x => x.UserId == member.UserId && x.DedupeKey == key && !x.IsRead)) continue;
            output.Add(new() { BusinessId = business, UserId = member.UserId, Type = e.Type, Title = e.Title, Message = e.Message, ReferenceType = e.Reference, ReferenceId = e.Id, DedupeKey = key });
        }
        return output;
    }
}
