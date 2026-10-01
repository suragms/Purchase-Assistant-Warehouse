using PurchaseAssistant.Domain.Common;
namespace PurchaseAssistant.Domain.Entities;
public class UserSettings : TenantEntity
{
    public Guid UserId { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public string NotificationKindsJson { get; set; } = "[\"low_stock\",\"delivery\",\"stock_variance\",\"staff_alert\",\"opening_stock\",\"physical_reminder\"]";
}
