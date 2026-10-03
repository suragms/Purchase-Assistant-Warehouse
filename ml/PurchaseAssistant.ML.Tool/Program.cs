using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.ML;
using System.Text.Json;

// Offline operator tool. Connection strings are accepted only through environment/local secrets, never arguments or logs.
if (args.Length == 0 || args[0] is not ("inspect" or "extract" or "train")) {
    Console.Error.WriteLine("Use inspect [--development-secrets], extract <business-id> <dataset.json> [--development-secrets], or train <dataset.json> <artifact-directory>."); return 2;
}
try {
    var command = args[0];
    if (command == "train") {
        if (args.Length != 3 || new FileInfo(args[1]).Length > 100_000_000) throw new ArgumentException("Invalid training input.");
        var dataset = JsonSerializer.Deserialize<TrainingDataset>(await File.ReadAllTextAsync(args[1])) ?? throw new InvalidDataException();
        if (dataset.BusinessId == Guid.Empty || dataset.ExtractedAt > DateTime.UtcNow || dataset.Items.Count > 10000) throw new InvalidDataException();
        var store = new ArtifactStore(args[2]); var trained = 0; var unavailable = 0;
        foreach (var item in dataset.Items) {
            var prepared = UsageData.Prepare(item.Observations, DateOnly.FromDateTime(dataset.ExtractedAt), dataset.ExtractedAt);
            if (prepared.Series.Count < UsageData.MinimumDays) { unavailable++; continue; }
            var artifact = ForecastModel.Train(dataset.BusinessId, item.ItemId, item.Unit, prepared, DateTime.UtcNow);
            await store.SaveAsync(artifact); trained++;
            Console.WriteLine(JsonSerializer.Serialize(new { item.ItemId, artifact.Model.Name, artifact.Version, artifact.ValidationMetrics, artifact.TestMetrics, artifact.BaselineTestMetrics, artifact.QualityAccepted, artifact.TrainingStart, artifact.TrainingEnd }));
        }
        Console.WriteLine(JsonSerializer.Serialize(new { trained, insufficientHistory = unavailable })); return trained > 0 ? 0 : 3;
    }
    var connection = Environment.GetEnvironmentVariable("ML_DATABASE");
    if (string.IsNullOrWhiteSpace(connection) && args.Contains("--development-secrets")) {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "UserSecrets", "PurchaseAssistant.WarehousePurchaseAssistant", "secrets.json");
        using var secrets = JsonDocument.Parse(await File.ReadAllTextAsync(path));
        connection = secrets.RootElement.TryGetProperty("ConnectionStrings:DefaultConnection", out var value) ? value.GetString() : null;
    }
    if (string.IsNullOrWhiteSpace(connection)) throw new ArgumentException("Configure ML_DATABASE or explicitly select local development secrets.");
    var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connection).Options;
    if (command == "inspect") {
        await using var db = new AppDbContext(options);
        // Metadata only. Does not read keys, personal records, stock values or purchase prices.
        Console.WriteLine(JsonSerializer.Serialize(new {
            businesses = await db.Businesses.CountAsync(), items = await db.CatalogItems.IgnoreQueryFilters().CountAsync(),
            usageRows = await db.DailyUsageLogs.IgnoreQueryFilters().CountAsync(),
            usageDates = await db.DailyUsageLogs.IgnoreQueryFilters().Select(x => x.Date).Distinct().CountAsync(),
            movements = await db.StockMovements.IgnoreQueryFilters().CountAsync(), purchases = await db.Purchases.IgnoreQueryFilters().CountAsync()
        })); return 0;
    }
    if (args.Length < 3 || !Guid.TryParse(args[1], out var business) || business == Guid.Empty) throw new ArgumentException("A business ID and output path are required.");
    await using var scoped = new AppDbContext(options, new Tenant(business));
    var now = DateTime.UtcNow; var from = DateOnly.FromDateTime(now).AddDays(-730);
    var catalog = await scoped.CatalogItems.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Take(10001).Select(x => new { x.Id, x.DefaultUnit }).ToListAsync();
    if (catalog.Count > 10000) throw new InvalidOperationException("Use a partitioned extraction for more than 10000 active items.");
    var output = new List<TrainingItem>();
    foreach (var item in catalog) {
        var observations = await scoped.DailyUsageLogs.AsNoTracking().Where(x => x.CatalogItemId == item.Id && x.Date >= from).OrderBy(x => x.Date).Take(1500)
            .Select(x => new UsageObservation(x.Date, (double?)x.UsedQty, x.IsConfirmed, x.LoggedAt)).ToListAsync();
        output.Add(new(item.Id, item.DefaultUnit, observations));
    }
    // Explicit output, no overwrite of a preexisting extraction.
    await using var file = new FileStream(Path.GetFullPath(args[2]), FileMode.CreateNew, FileAccess.Write, FileShare.None);
    await JsonSerializer.SerializeAsync(file, new TrainingDataset(business, now, output));
    Console.WriteLine(JsonSerializer.Serialize(new { items = output.Count, observations = output.Sum(x => x.Observations.Count) })); return 0;
} catch (Exception ex) when (ex is ArgumentException or IOException or InvalidOperationException or JsonException or Npgsql.NpgsqlException) {
    Console.Error.WriteLine("ML operation failed. Check input, database schema/access and storage configuration. No connection or record details are logged."); return 1;
}

record TrainingDataset(Guid BusinessId, DateTime ExtractedAt, List<TrainingItem> Items);
record TrainingItem(Guid ItemId, string Unit, List<UsageObservation> Observations);
sealed class Tenant(Guid business) : ITenantProvider { public Guid GetBusinessId() => business; }
