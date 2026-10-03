using System.Security.Cryptography;
using System.Text.Json;

namespace PurchaseAssistant.ML;

public class ArtifactStore(string? directory)
{
    public bool Configured => !string.IsNullOrWhiteSpace(directory);
    private string PathFor(Guid business, Guid item) => Path.Combine(Path.GetFullPath(directory ?? throw new InvalidOperationException("Model storage is not configured.")), business.ToString("N"), item.ToString("N") + ".json");
    private record Envelope(string Sha256, string Payload);
    public async Task SaveAsync(ModelArtifact artifact, CancellationToken ct = default)
    {
        Validate(artifact, artifact.BusinessId, artifact.ItemId);
        var path = PathFor(artifact.BusinessId, artifact.ItemId); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var payload = JsonSerializer.Serialize(artifact); var envelope = JsonSerializer.Serialize(new Envelope(Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(payload))), payload));
        var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try { await File.WriteAllTextAsync(temp, envelope, ct); File.Move(temp, path, true); }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }
    public async Task<ModelArtifact?> LoadAsync(Guid business, Guid item, CancellationToken ct = default)
    {
        if (!Configured) return null;
        var path = PathFor(business, item); if (!File.Exists(path)) return null;
        if (new FileInfo(path).Length > 1_000_000) throw new InvalidDataException("Invalid model artifact.");
        try {
            var envelope = JsonSerializer.Deserialize<Envelope>(await File.ReadAllTextAsync(path, ct)) ?? throw new InvalidDataException();
            if (envelope.Payload == null || Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(envelope.Payload))) != envelope.Sha256) throw new InvalidDataException();
            var model = JsonSerializer.Deserialize<ModelArtifact>(envelope.Payload) ?? throw new InvalidDataException();
            Validate(model, business, item); return model;
        } catch (Exception ex) when (ex is JsonException or NullReferenceException or ArgumentException) { throw new InvalidDataException("Invalid model artifact."); }
    }
    public static void Validate(ModelArtifact a, Guid business, Guid item)
    {
        if (a.SchemaVersion != 1 || a.BusinessId != business || a.ItemId != item || a.FeatureVersion != UsageData.FeatureVersion
            || !ForecastModel.Candidates.Contains(a.Model.Name) || a.TrainingHistory.Count is < 120 or > 730
            || a.DatasetVersion != UsageData.Fingerprint(a.TrainingHistory) || !double.IsFinite(a.AbsoluteError90) || a.AbsoluteError90 < 0
            || a.TrainingStart != a.TrainingHistory[0].Date || a.TrainingEnd != a.TrainingHistory[^1].Date
            || a.TrainingHistory.Any(x => !double.IsFinite(x.Quantity) || x.Quantity is < 0 or > 1_000_000_000)
            || a.TrainingHistory.Zip(a.TrainingHistory.Skip(1)).Any(x => x.Second.Date != x.First.Date.AddDays(1))) throw new InvalidDataException("Invalid model artifact.");
        if (a.Model.Name == "ridge" && (a.Model.Coefficients.Length != 8 || a.Model.Means.Length != 7 || a.Model.Scales.Length != 7
            || a.Model.Coefficients.Concat(a.Model.Means).Concat(a.Model.Scales).Any(x => !double.IsFinite(x)) || a.Model.Scales.Any(x => x <= 0))) throw new InvalidDataException("Invalid model coefficients.");
    }
}
