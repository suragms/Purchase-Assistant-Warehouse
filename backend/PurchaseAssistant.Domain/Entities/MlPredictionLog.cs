using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities;

public class MlPredictionLog : TenantEntity
{
    public Guid CatalogItemId { get; set; }
    public Guid UserId { get; set; }
    public string ModelVersion { get; set; } = "";
    public string InputVersion { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public int Horizon { get; set; }
    public decimal PredictedQuantity { get; set; }
    public string DailyPredictionsJson { get; set; } = "[]";
}
