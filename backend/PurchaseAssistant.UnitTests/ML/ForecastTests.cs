using PurchaseAssistant.ML;
using Xunit;

namespace PurchaseAssistant.UnitTests.ML;

public class ForecastTests
{
    public static readonly DateOnly Today = new(2026, 10, 3);
    public static readonly DateTime Now = new(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc);
    public static List<UsageObservation> Observations(int count = 180) => Enumerable.Range(0, count).Select(i => {
        var date = Today.AddDays(i - count); return new UsageObservation(date, 20 + i * .15 + (int)date.DayOfWeek * 2, true, date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc)); }).ToList();
    public static ModelArtifact Train(List<UsageObservation>? rows = null) => ForecastModel.Train(Guid.Parse("11111111-1111-1111-1111-111111111111"), Guid.Parse("22222222-2222-2222-2222-222222222222"), "PCS", UsageData.Prepare(rows ?? Observations(), Today, Now), Now);

    [Fact] public void CleaningRejectsInvalidAndFutureRecordsWithoutInventingZeros()
    {
        var rows = Observations();
        rows.AddRange([new(Today, 5, true, Now), new(Today.AddDays(-181), -1, true, Now), new(Today.AddDays(-182), null, true, Now),
            new(Today.AddDays(-183), double.NaN, true, Now), new(Today.AddDays(-184), 1, true, Now.AddDays(1)), new(Today.AddDays(-185), 0, false, Now)]);
        var data = UsageData.Prepare(rows, Today, Now); Assert.Equal(6, data.Rejected); Assert.Equal(180, data.Series.Count);
    }
    [Fact] public void MissingDayBreaksHistoryAndConfirmedZerosRemainValid()
    {
        var rows = Observations(); rows.RemoveAt(140); rows[^1] = rows[^1] with { Quantity = 0 };
        var data = UsageData.Prepare(rows, Today, Now); Assert.Equal(39, data.Series.Count); Assert.Equal(1, data.MissingDays); Assert.Equal(0, data.Series[^1].Quantity);
        Assert.Throws<ArgumentException>(() => ForecastModel.Train(Guid.NewGuid(), Guid.NewGuid(), "PCS", data, Now));
    }
    [Fact] public void IdenticalDuplicatesCollapseButConflictingDuplicatesAreUnknown()
    {
        var rows = Observations(); rows.Add(rows[^1]); Assert.Equal(180, UsageData.Prepare(rows, Today, Now).Series.Count);
        rows.Add(rows[^1] with { Quantity = 999 }); Assert.Empty(UsageData.Prepare(rows, Today, Now).Series);
    }
    [Fact] public void FeaturesUseOnlyPastObservations()
    {
        var history = Enumerable.Range(1, 28).Select(i => (double)i).ToList();
        var features = ForecastModel.Features(history, Today);
        Assert.Equal(28, features[0]); Assert.Equal(22, features[1]); Assert.Equal(25, features[3]); Assert.Equal(14.5, features[4]);
    }
    [Fact] public void TrainingIsReproducibleAndHoldoutCannotAffectModelSelection()
    {
        var rows = Observations(); var first = Train(rows); var second = Train(rows);
        Assert.Equal(first.Version, second.Version); Assert.Equal(first.Model.Coefficients, second.Model.Coefficients);
        var modified = rows.Select((r, i) => i >= rows.Count - 30 ? r with { Quantity = 10000 } : r).ToList(); var changed = Train(modified);
        Assert.Equal(first.Model.Name, changed.Model.Name); Assert.Equal(first.ValidationMetrics, changed.ValidationMetrics);
        Assert.NotEqual(first.TestMetrics, changed.TestMetrics); Assert.True(first.ValidationEnd < first.TestStart);
        Assert.True(first.TestMetrics.Mae < first.BaselineTestMetrics.Mae); Assert.Equal("ridge", first.Model.Name);
    }
    [Theory] [InlineData(7)] [InlineData(14)] [InlineData(30)]
    public void ForecastIsRecursiveFiniteAndStartsAfterHistory(int horizon)
    {
        var artifact = Train(); var forecast = ForecastModel.Predict(artifact.Model, artifact.TrainingHistory, horizon);
        Assert.Equal(horizon, forecast.Count); Assert.Equal(Today, forecast[0].Date); Assert.All(forecast, x => Assert.True(double.IsFinite(x.Quantity) && x.Quantity >= 0));
    }
    [Theory] [InlineData(0)] [InlineData(31)] public void InvalidHorizonsRejected(int horizon) { var a = Train(); Assert.Throws<ArgumentException>(() => ForecastModel.Predict(a.Model, a.TrainingHistory, horizon)); }
    [Fact] public void ZeroDemandMetricsHaveNoDivideByZeroOrFalseAccuracyScore()
    {
        var a = Train(Observations().Select(r => r with { Quantity = 0 }).ToList());
        Assert.Null(a.TestMetrics.Wape); Assert.Null(a.TestMetrics.Mape); Assert.Null(a.TestMetrics.RSquared); Assert.Equal(0, a.TestMetrics.Mae);
        Assert.All(ForecastModel.Predict(a.Model, a.TrainingHistory, 30), p => Assert.Equal(0, p.Quantity));
    }
    [Fact] public async Task ArtifactsRoundTripAndRejectCorruptionTenantMismatchAndInvalidCoefficients()
    {
        var root = Path.Combine(Path.GetTempPath(), "wa-ml-tests-" + Guid.NewGuid().ToString("N"));
        try {
            var a = Train(); var store = new ArtifactStore(root); Assert.Null(await store.LoadAsync(a.BusinessId, a.ItemId));
            await store.SaveAsync(a); var loaded = await store.LoadAsync(a.BusinessId, a.ItemId); Assert.Equal(a.Version, loaded!.Version);
            Assert.Throws<InvalidDataException>(() => ArtifactStore.Validate(a, Guid.NewGuid(), a.ItemId));
            Assert.Throws<InvalidDataException>(() => ArtifactStore.Validate(a with { Model = a.Model with { Scales = [0] } }, a.BusinessId, a.ItemId));
            var path = Path.Combine(root, a.BusinessId.ToString("N"), a.ItemId.ToString("N") + ".json");
            await File.WriteAllTextAsync(path, "{bad json}"); await Assert.ThrowsAsync<InvalidDataException>(() => store.LoadAsync(a.BusinessId, a.ItemId));
        } finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
