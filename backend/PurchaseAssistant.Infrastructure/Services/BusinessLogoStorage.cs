using PurchaseAssistant.Application.Interfaces;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace PurchaseAssistant.Infrastructure.Services;
// Only enabled for a configured durable directory or local development. Production object storage remains an explicit deployment integration.
public class BusinessLogoStorage(string? root) : IBusinessLogoStorage
{
    public bool Available => !string.IsNullOrWhiteSpace(root);
    private string PathFor(Guid business, string key)
    {
        if (!Available) throw new NotSupportedException("Logo storage is not configured.");
        if (!Regex.IsMatch(key, "^[A-F0-9]{64}\\.png$")) throw new ArgumentException("Invalid storage key.");
        return Path.Combine(Path.GetFullPath(root!), business.ToString("N"), key);
    }
    public async Task<string> StoreAsync(Guid business, byte[] png, CancellationToken ct)
    {
        var key = Convert.ToHexString(SHA256.HashData(png)) + ".png"; var path = PathFor(business, key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        // Content-addressed files are immutable; concurrent equal uploads have identical bytes.
        if (!File.Exists(path)) { var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp"; try { await File.WriteAllBytesAsync(temp, png, ct); File.Move(temp, path, true); } finally { if (File.Exists(temp)) File.Delete(temp); } }
        return key;
    }
    public Task<byte[]?> ReadAsync(Guid business, string key, CancellationToken ct)
    {
        var path = PathFor(business, key); return File.Exists(path) ? Read(path, ct) : Task.FromResult<byte[]?>(null);
    }
    private static async Task<byte[]?> Read(string path, CancellationToken ct) => await File.ReadAllBytesAsync(path, ct);
    public Task DeleteAsync(Guid business, string key, CancellationToken ct) { var path = PathFor(business, key); if (File.Exists(path)) File.Delete(path); return Task.CompletedTask; }
}
