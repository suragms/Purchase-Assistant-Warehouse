using PurchaseAssistant.Domain.Common;
namespace PurchaseAssistant.Domain.Entities;
public class ProviderCredential : TenantEntity
{
    public string CredentialType { get; set; } = "";
    public string EncryptedValue { get; set; } = "";
    public string LastFour { get; set; } = "";
    public Guid UpdatedById { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
}
public class UserSettings : TenantEntity
{
    public Guid UserId { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public string NotificationKindsJson { get; set; } = "[\"low_stock\",\"delivery\",\"stock_variance\",\"staff_alert\",\"opening_stock\",\"physical_reminder\"]";
}
