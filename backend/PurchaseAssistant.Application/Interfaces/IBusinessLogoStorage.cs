namespace PurchaseAssistant.Application.Interfaces;
public interface IBusinessLogoStorage
{
    bool Available { get; }
    Task<string> StoreAsync(Guid businessId, byte[] png, CancellationToken ct);
    Task<byte[]?> ReadAsync(Guid businessId, string key, CancellationToken ct);
    Task DeleteAsync(Guid businessId, string key, CancellationToken ct);
}
