using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PurchaseAssistant.Application.DTOs.AI;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Infrastructure.Services.AI;
namespace PurchaseAssistant.IntegrationTests.Stock;
public partial class StockServiceIntegrationTests
{
    private sealed class BackupConfiguration(string root) : IConfiguration
    {
        public string? this[string key] { get => key == "BACKUP_DIR" ? root : null; set => throw new NotSupportedException(); }
        public IEnumerable<IConfigurationSection> GetChildren() => [];
        public IConfigurationSection GetSection(string key) => throw new NotSupportedException();
        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken() => throw new NotSupportedException();
    }
    [Fact]
    public async Task BusinessBackupsStreamPostgresDataRetainFourteenFilesAndNeverDeleteHistoryOrArbitraryPaths()
    {
        var root = Path.Combine(Path.GetTempPath(), "warehouse-backup-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root); var sentinel = Path.Combine(root, "sentinel.txt"); await File.WriteAllTextAsync(sentinel, "PRIVATE");
        try {
            var item = await CreateItemAsync(current: 12.3456m);
            var service = new BusinessBackupService(_context, new BackupConfiguration(root), TimeProvider.System);
            _context.BackupLogs.Add(new BackupLog { BusinessId = _businessId, Status = "success", FilePath = "../sentinel.txt", CreatedAt = DateTime.UtcNow.AddDays(-30) }); await _context.SaveChangesAsync();
            for (var i = 0; i < 15; i++) {
                var log = await service.RunAsync(_businessId, i == 0 ? "scheduled" : "manual", i == 0 ? null : _userId, default);
                Assert.Equal("success", log.Status); Assert.NotNull(log.SizeBytes);
                if (i == 0) { using var payload = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, _businessId.ToString(), log.FilePath!)));
                    Assert.Equal(_businessId, payload.RootElement.GetProperty("business_id").GetGuid()); Assert.Equal(12.3456m, payload.RootElement.GetProperty("catalog")[0].GetProperty("current_stock").GetDecimal()); }
            }
            Assert.Equal(14, Directory.GetFiles(Path.Combine(root, _businessId.ToString()), "*.json").Length);
            Assert.Equal(16, (await service.HistoryAsync(_businessId, default)).Count); Assert.Equal("PRIVATE", await File.ReadAllTextAsync(sentinel));
            Assert.Equal(15, await _context.SecurityAuditLogs.CountAsync(x => x.EventType == "backup_run"));
            var saved = await _context.BackupLogs.FirstAsync(); saved.Status = "changed";
            await Assert.ThrowsAsync<InvalidOperationException>(() => _context.SaveChangesAsync()); _context.Entry(saved).State = EntityState.Detached;
            Assert.Equal(12.3456m, (await _context.CatalogItems.SingleAsync(x => x.Id == item.Id)).CurrentStock);
        } finally {
            var allowed = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "warehouse-backup-tests")) + Path.DirectorySeparatorChar;
            if (Path.GetFullPath(root).StartsWith(allowed, StringComparison.OrdinalIgnoreCase)) Directory.Delete(root, true);
        }
    }
    [Fact]
    public async Task AiUsagePersistsOnlySafeMetadataAndCannotBeEdited()
    {
        var recorder = new AiUsageRecorder(_context, _user);
        await recorder.RecordAsync(new AIResponse(true, "PRIVATE CUSTOMER BODY", "private-api-key", "OpenAI", "private-token-model", 25.7m), true, default);
        var log = await _context.AiUsageLogs.SingleAsync(); Assert.Equal("openai", log.Provider); Assert.Equal(25, log.LatencyMs); Assert.True(log.Escalated); Assert.Null(log.Model);
        var json = JsonSerializer.Serialize(log); Assert.DoesNotContain("PRIVATE", json); Assert.DoesNotContain("private", json);
        log.Provider = "changed"; await Assert.ThrowsAsync<InvalidOperationException>(() => _context.SaveChangesAsync());
    }
    [Fact]
    public async Task BackupHistoryRejectsANonexistentBusinessForeignKey()
    {
        _context.BackupLogs.Add(new BackupLog { BusinessId = Guid.NewGuid(), Status = "fail" });
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync()); _context.ChangeTracker.Clear();
    }
}
