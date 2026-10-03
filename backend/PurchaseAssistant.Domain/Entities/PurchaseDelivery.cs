using PurchaseAssistant.Domain.Common;
namespace PurchaseAssistant.Domain.Entities;
public class PurchaseDelivery : TenantEntity
{
    public Guid PurchaseId { get; set; }
    public Guid RequestId { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
    public string Status { get; set; } = "sending";
    public string RecipientLastFour { get; set; } = "";
    public string? MessageId { get; set; }
    public string? ErrorCode { get; set; }
    public int Attempts { get; set; }
}
