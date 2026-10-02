using PurchaseAssistant.Domain.Common;

namespace PurchaseAssistant.Domain.Entities;

// Source contracts: owner_ops.BackupLog and owner_ops.AiUsageLog.
public class BackupLog : TenantEntity
{
    public string RunType { get; set; } = "manual";
    public string Status { get; set; } = "failed";
    public string? FilePath { get; set; }
    public long? SizeBytes { get; set; }
    public string RowCountsJson { get; set; } = "{}";
    public int? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AiUsageLog : TenantEntity
{
    public string Feature { get; set; } = "json_extract";
    public string Endpoint { get; set; } = "llm_failover";
    public string Provider { get; set; } = "none";
    public string? Model { get; set; }
    public int? LatencyMs { get; set; }
    public bool Escalated { get; set; }
}
