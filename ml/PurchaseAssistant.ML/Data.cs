using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace PurchaseAssistant.ML;

public record UsageObservation(DateOnly Date, double? Quantity, bool Confirmed, DateTime RecordedAt);
public record DailyValue(DateOnly Date, double Quantity);
public record PreparedData(List<DailyValue> Series, int Rejected, int Duplicates, int MissingDays, string Version);

public static class UsageData
{
    public const string FeatureVersion = "daily-confirmed-usage-v1";
    public const int MinimumDays = 120;
    public static PreparedData Prepare(IEnumerable<UsageObservation> observations, DateOnly asOf, DateTime extractedAt)
    {
        var rows = observations.ToList();
        var valid = rows.Where(x => x.Confirmed && x.Date < asOf && x.Date >= asOf.AddDays(-730)
            && x.Quantity is >= 0 and <= 1_000_000_000 && double.IsFinite(x.Quantity.Value)
            && x.RecordedAt != default && x.RecordedAt <= extractedAt
            && DateOnly.FromDateTime(x.RecordedAt) == x.Date)
            .Select(x => x with { Quantity = Math.Round(x.Quantity!.Value, 4, MidpointRounding.AwayFromZero) }).ToList();
        var groups = valid.GroupBy(x => x.Date).ToList();
        // Daily entries are cumulative. Identical duplicates collapse; conflicting duplicates are unknown.
        var days = groups.Where(g => g.Select(x => x.Quantity).Distinct().Count() == 1)
            .Select(g => new DailyValue(g.Key, g.First().Quantity!.Value)).OrderBy(x => x.Date).ToList();
        var duplicateCount = groups.Sum(g => g.Count() - 1);
        var missing = days.Count == 0 ? 0 : days[^1].Date.DayNumber - days[0].Date.DayNumber + 1 - days.Count;
        // Only the contiguous suffix ending yesterday is eligible. Never fill unknown days with zeros.
        var suffix = new List<DailyValue>();
        var expected = asOf.AddDays(-1);
        for (var i = days.Count - 1; i >= 0 && days[i].Date == expected; i--, expected = expected.AddDays(-1)) suffix.Add(days[i]);
        suffix.Reverse();
        return new(suffix, rows.Count - valid.Count + groups.Where(g => g.Select(x => x.Quantity).Distinct().Count() > 1).Sum(g => g.Count()),
            duplicateCount, missing, Fingerprint(suffix));
    }
    public static string Fingerprint(IEnumerable<DailyValue> values) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
        string.Join("\n", values.Select(x => $"{x.Date:yyyy-MM-dd},{Math.Round(x.Quantity, 4, MidpointRounding.AwayFromZero).ToString("F4", CultureInfo.InvariantCulture)}"))))).ToLowerInvariant();
}
