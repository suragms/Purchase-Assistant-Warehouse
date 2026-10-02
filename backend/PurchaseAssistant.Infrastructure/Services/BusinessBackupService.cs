using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;

namespace PurchaseAssistant.Infrastructure.Services;

public record BackupLogDto(Guid Id, string RunType, string Status, string? FilePath, long? SizeBytes,
    Dictionary<string, int> RowCounts, int? DurationMs, string? ErrorMessage, DateTime CreatedAt);

public class BusinessBackupService(AppDbContext db, IConfiguration configuration, TimeProvider clock)
{
    public const string SchemaVersion = "harisree-backup-v1";
    public string Root => Path.GetFullPath(configuration["BACKUP_DIR"]
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HarisreeBackups"));
    private static bool SafeName(string? name) => name != null && Regex.IsMatch(name,
        @"\Abackup_\d{8}T\d{6}Z_[a-f0-9]{32}\.json\z", RegexOptions.CultureInvariant);
    public static BackupLogDto Metadata(BackupLog x) => new(x.Id, x.RunType, x.Status,
        SafeName(x.FilePath) ? x.FilePath : null, x.SizeBytes,
        JsonSerializer.Deserialize<Dictionary<string, int>>(x.RowCountsJson) ?? new(),
        x.DurationMs, x.Status == "fail" ? "Business backup could not be completed." : null, x.CreatedAt);

    public async Task<List<BackupLogDto>> HistoryAsync(Guid businessId, CancellationToken ct)
        => (await db.BackupLogs.AsNoTracking().Where(x => x.BusinessId == businessId)
            .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(50).ToListAsync(ct)).Select(Metadata).ToList();

    public async Task<BackupLogDto> RunAsync(Guid businessId, string runType, Guid? actorId, CancellationToken ct)
    {
        if (runType is not ("manual" or "scheduled")) throw new ArgumentException("Invalid backup run type.");
        if (!await db.Businesses.AnyAsync(x => x.Id == businessId && x.IsActive, ct)) throw new KeyNotFoundException("Business not found.");
        var log = new BackupLog { BusinessId = businessId, RunType = runType, Status = "fail", CreatedAt = clock.GetUtcNow().UtcDateTime };
        var watch = Stopwatch.StartNew(); string? file = null;
        try
        {
            var directory = Path.Combine(Root, businessId.ToString("D"));
            Directory.CreateDirectory(directory);
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new IOException();
            var name = $"backup_{log.CreatedAt:yyyyMMddTHHmmssZ}_{log.Id:N}.json";
            file = Path.Combine(directory, name);
            await using var snapshot = db.Database.IsRelational() ? await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, ct) : null;
            // Explicit business predicates are mandatory: the scheduled worker has no selected-user tenant.
            var catalog = db.CatalogItems.IgnoreQueryFilters().AsNoTracking().Where(x => x.BusinessId == businessId && x.IsActive);
            var suppliers = db.Suppliers.IgnoreQueryFilters().AsNoTracking().Where(x => x.BusinessId == businessId);
            var purchases = ReportService.ReportingPurchases(db, businessId).IgnoreQueryFilters();
            var audits = db.StockMovements.IgnoreQueryFilters().AsNoTracking().Where(x => x.BusinessId == businessId)
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(2000);
            var counts = new Dictionary<string, int> { ["catalog"] = await catalog.CountAsync(ct), ["suppliers"] = await suppliers.CountAsync(ct),
                ["purchases"] = await purchases.CountAsync(ct), ["stock_audits"] = await audits.CountAsync(ct) };
            await using (var stream = new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, true))
            {
                using var writer = new Utf8JsonWriter(stream);
                writer.WriteStartObject(); writer.WriteString("schema_version", SchemaVersion); writer.WriteString("business_id", businessId); writer.WriteString("exported_at", log.CreatedAt);
                writer.WriteStartArray("excludes"); writer.WriteStringValue("provider_credentials"); writer.WriteEndArray();
                // Serialize each query fully before opening the next PostgreSQL reader.
                async Task Array<T>(string key, IAsyncEnumerable<T> rows) {
                    writer.WriteStartArray(key); var n = 0;
                    await foreach (var row in rows.WithCancellation(ct)) { JsonSerializer.Serialize(writer, row); if (++n % 128 == 0) await writer.FlushAsync(ct); }
                    writer.WriteEndArray(); await writer.FlushAsync(ct);
                }
                await Array("catalog", catalog.OrderBy(x => x.Id).Select(x => new { id = x.Id, name = x.Name, code = x.ItemCode, barcode = x.Barcode, unit = x.DefaultUnit, current_stock = x.CurrentStock }).AsAsyncEnumerable());
                await Array("suppliers", suppliers.OrderBy(x => x.Id).Select(x => new { id = x.Id, name = x.Name }).AsAsyncEnumerable());
                await Array("purchases", purchases.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Select(x => new { id = x.Id, date = x.CreatedAt,
                    status = x.Status.ToString(), supplier_id = x.SupplierId,
                    lines = x.Items.Where(i => i.BusinessId == businessId).OrderBy(i => i.Id).Select(i => new { id = i.Id, catalog_item_id = i.CatalogItemId, qty = i.OrderedQuantity }).ToArray() }).AsAsyncEnumerable());
                // The target's immutable stock ledger supplies the reference audit identifiers/timestamps.
                await Array("stock_audits", audits.Select(x => new { id = x.Id, created_at = x.CreatedAt }).AsAsyncEnumerable());
                writer.WriteEndObject(); await writer.FlushAsync(ct);
            }
            if (snapshot != null) await snapshot.CommitAsync(ct);
            log.Status = "success"; log.FilePath = name; log.SizeBytes = new FileInfo(file).Length;
            log.RowCountsJson = JsonSerializer.Serialize(counts);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { TryDeletePartial(file); throw; }
        catch (Exception) { TryDeletePartial(file); log.ErrorMessage = "Business backup could not be completed."; }
        log.DurationMs = (int)Math.Min(watch.ElapsedMilliseconds, int.MaxValue);
        db.BackupLogs.Add(log);
        db.SecurityAuditLogs.Add(new SecurityAuditLog { BusinessId = businessId, UserId = actorId,
            EventType = "backup_run", Description = $"{runType} business backup: {log.Status}", MetadataJson = JsonSerializer.Serialize(new { backupLogId = log.Id }) });
        await db.SaveChangesAsync(ct);
        if (log.Status == "success") await ApplyRetentionAsync(businessId, ct);
        return Metadata(log);
    }
    private static void TryDeletePartial(string? file) { if (file == null) return; try { File.Delete(file); } catch (IOException) { } catch (UnauthorizedAccessException) { } }
    public async Task ApplyRetentionAsync(Guid businessId, CancellationToken ct)
    {
        var directory = Path.Combine(Root, businessId.ToString("D"));
        if (!Directory.Exists(directory) || (File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) return;
        var older = await db.BackupLogs.IgnoreQueryFilters().AsNoTracking().Where(x => x.BusinessId == businessId && x.Status == "success")
            .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Skip(14).Select(x => x.FilePath).ToListAsync(ct);
        foreach (var name in older.Where(SafeName))
        {
            var file = Path.Combine(directory, name!);
            try { if (File.Exists(file) && (File.GetAttributes(file) & FileAttributes.ReparsePoint) == 0) File.Delete(file); }
            catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
        // Retention removes old files, never historical run records.
    }
}
