using System.ComponentModel.DataAnnotations;
namespace PurchaseAssistant.Application.DTOs.Settings;
public class BusinessProfileDto
{
    [Required, StringLength(255)] public string Name { get; set; } = "";
    [StringLength(128)] public string? BrandingTitle { get; set; }
    [StringLength(512)] public string? BrandingLogoUrl { get; set; }
    [StringLength(20)] public string? GstNumber { get; set; }
    [StringLength(2000)] public string? Address { get; set; }
    [StringLength(32)] public string? Phone { get; set; }
    [EmailAddress, StringLength(255)] public string? ContactEmail { get; set; }
    public Guid Version { get; set; }
    public bool HasUploadedLogo { get; set; }
    public bool LogoUploadAvailable { get; set; }
}
public class UserSettingsDto
{
    public bool NotificationsEnabled { get; set; } = true;
    public string[] NotificationKinds { get; set; } = ["low_stock", "delivery", "stock_variance", "staff_alert", "opening_stock", "physical_reminder"];
}
