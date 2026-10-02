using System.Text.Json;
using Moq;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Application.Services;
using PurchaseAssistant.Infrastructure.Services;

namespace PurchaseAssistant.UnitTests.Reports;
public class HistoricalMetadataValidatorTests
{
    private static readonly Guid Business = Guid.NewGuid(), Actor = Guid.NewGuid();
    private static ICurrentUserService User(string role = "Owner", Guid? business = null)
    {
        var user = new Mock<ICurrentUserService>(); user.SetupGet(x => x.BusinessId).Returns(business ?? Business);
        user.SetupGet(x => x.UserId).Returns(Actor); user.SetupGet(x => x.Role).Returns(role);
        user.Setup(x => x.HasPermission(It.IsAny<string>())).Returns(true); return user.Object;
    }
    public static IEnumerable<object[]> FixtureCases()
    {
        var data = HistoricalPreviewFixtures.Create("mixed", Business, Actor);
        var valid = new[] { "fully-valid", "known-zero-rate", "valid-name", "valid-unit", "valid-weight", "unicode-name", "decimal-quantity", "decimal-rate", "zero-opening-quantity", "not-captured-rate", "not-applicable-weight" };
        foreach (var row in data.Input.Rows)
        {
            var expected = valid.Contains(row.Case) ? "VALID" : row.Case is "missing-opening" or "missing-date" or "missing-rate" or "missing-name" or "blank-unit" ? "WARNING" :
                row.Case is "ambiguous-item-code" or "conflicting-barcode" ? "AMBIGUOUS" : row.Case == "item-not-found" ? "NOT_FOUND" : "REJECTED";
            yield return [row.Case, expected];
        }
    }
    [Theory, MemberData(nameof(FixtureCases))]
    public void EverySyntheticCaseProducesTheExpectedIndependentOutcome(string name, string expected)
    {
        var data = HistoricalPreviewFixtures.Create("mixed", Business, Actor);
        var result = HistoricalMetadataValidator.Preview(data.Input, data.Targets, User());
        var row = Assert.Single(result.Rows, x => x.Case == name); Assert.Equal(expected, row.Outcome);
        Assert.Equal(8, row.Fields.Count); Assert.All(row.Fields, f => Assert.Contains(f.State, new[] { "KNOWN", "UNKNOWN", "NOT_APPLICABLE", "NOT_CAPTURED" }));
        Assert.False(result.WritesPerformed); Assert.False(result.PersistenceAvailable); Assert.False(result.ConfirmationAvailable);
        if (row.Match != "MATCHED" || row.Duplicate || row.Provenance == null) Assert.All(row.Fields, f => Assert.Null(f.ProposedValue));
        if (row.Match != "MATCHED") { Assert.Empty(row.UnchangedCurrentValues); Assert.Null(row.MatchedItemId); }
    }
    [Fact]
    public void MissingAndOptionalStatesStayNullWhileKnownZeroSurvivesAndCurrentValuesAreNeverSubstituted()
    {
        var data = HistoricalPreviewFixtures.Create("mixed", Business, Actor); var result = HistoricalMetadataValidator.Preview(data.Input, data.Targets, User());
        Assert.Equal("0", result.Rows.Single(x => x.Case == "known-zero-rate").Fields.Single(x => x.Field == "sellingRate").ProposedValue);
        Assert.Equal("0", result.Rows.Single(x => x.Case == "zero-opening-quantity").Fields.Single(x => x.Field == "openingStock").ProposedValue);
        foreach (var (name, field) in new[] { ("missing-opening", "openingStock"), ("missing-rate", "sellingRate"), ("missing-name", "historicalName"), ("blank-unit", "historicalUnit"), ("missing-weight", "kgPerUnit") })
        { var f = result.Rows.Single(x => x.Case == name).Fields.Single(x => x.Field == field); Assert.Equal("UNKNOWN", f.State); Assert.Null(f.ProposedValue); }
        Assert.Equal("NOT_CAPTURED", result.Rows.Single(x => x.Case == "not-captured-rate").Fields.Single(x => x.Field == "sellingRate").State);
        Assert.Equal("NOT_APPLICABLE", result.Rows.Single(x => x.Case == "not-applicable-weight").Fields.Single(x => x.Field == "kgPerUnit").State);
        Assert.Equal("Synthetic current item", result.Rows.Single(x => x.Case == "valid-name").UnchangedCurrentValues["catalogName"]);
    }
    [Theory, InlineData("Manager"), InlineData("Staff"), InlineData("Admin")]
    public void ForgedPermissionsDoNotGrantPrivilegedPreview(string role)
    { var data = HistoricalPreviewFixtures.Create("valid", Business, Actor); Assert.Throws<UnauthorizedAccessException>(() => HistoricalMetadataValidator.Preview(data.Input, data.Targets, User(role))); }
    [Theory, InlineData("Owner"), InlineData("SuperAdmin")]
    public void PrivilegedPreviewIsScopedAndDeterministicWithoutMutatingInput(string role)
    {
        var data = HistoricalPreviewFixtures.Create("mixed", Business, Actor); var before = JsonSerializer.Serialize(data);
        var a = HistoricalMetadataValidator.Preview(data.Input, data.Targets, User(role)); var b = HistoricalMetadataValidator.Preview(data.Input, data.Targets, User(role));
        Assert.Equal(JsonSerializer.Serialize(a), JsonSerializer.Serialize(b)); Assert.Equal(before, JsonSerializer.Serialize(data));
        Assert.True(a.Summary.TotalRows >= 30); Assert.Equal(a.Summary.TotalRows, a.Summary.ValidRows + a.Summary.WarningRows + a.Summary.RejectedRows + a.Summary.AmbiguousRows + a.Summary.NotFoundRows);
        Assert.Equal(4, a.Summary.DuplicateRows);
        Assert.Throws<ArgumentException>(() => HistoricalMetadataValidator.Preview(data.Input with { BusinessId = Guid.NewGuid() }, data.Targets, User(role)));
        Assert.Throws<ArgumentException>(() => HistoricalMetadataValidator.Preview(data.Input with { Synthetic = false }, data.Targets, User(role)));
    }
    public static IEnumerable<object[]> InvalidFields => new[] {
        new[] { "openingStock", "KNOWN", "1e3" }, new[] { "sellingRate", "KNOWN", "1.001" }, new[] { "historicalName", "KNOWN", "Name\nInjected" },
        new[] { "historicalName", "KNOWN", "Name\u202eHidden" }, new[] { "historicalUnit", "KNOWN", "BASKET" }, new[] { "kgPerUnit", "KNOWN", "0" },
        new[] { "openingStock", "INVALID", "1" }, new[] { "openingStock", "UNKNOWN", "0" }, new[] { "historicalName", "NOT_CAPTURED", "" },
        new[] { "openingStock", "NOT_APPLICABLE", "" }, new[] { "totalWeight", "KNOWN", "10" }, new[] { "businessDate", "KNOWN", "02/01/2020" }
    }.Select(x => x.Cast<object>().ToArray());
    [Theory, MemberData(nameof(InvalidFields))]
    public void UnsafeFieldValuesAreRejectedWithoutDefaulting(string field, string state, string raw)
    {
        var data = HistoricalPreviewFixtures.Create("valid", Business, Actor); var row = data.Input.Rows[0];
        var fields = row.Fields.ToDictionary(x => x.Key, x => x.Value); fields[field] = new(state, raw == "" ? null : raw, raw == "" ? null : raw);
        var result = HistoricalMetadataValidator.Preview(data.Input with { Rows = [row with { Fields = fields }] }, data.Targets, User());
        Assert.Equal("REJECTED", Assert.Single(result.Rows).Outcome); Assert.Null(result.Rows[0].Fields.Single(x => x.Field == field).ProposedValue);
    }
    [Theory, InlineData("invoice_date"), InlineData("created_at"), InlineData("received_at"), InlineData("completed_at")]
    public void OtherDateSemanticsCannotSubstituteForCalendarPurchaseDate(string meaning)
    { var data = HistoricalPreviewFixtures.Create("valid", Business, Actor); var r = data.Input.Rows[0]; Assert.Equal("REJECTED", HistoricalMetadataValidator.Preview(data.Input with { Rows = [r with { DateMeaning = meaning }] }, data.Targets, User()).Rows[0].Outcome); }
    [Fact]
    public void ConflictingSourceMappingsAndSourceTenantMismatchNeverSelectATarget()
    {
        var data = HistoricalPreviewFixtures.Create("valid", Business, Actor); var first = data.Input.Rows[0];
        var conflict = data.Input.SourceMap.Append(data.Input.SourceMap[0] with { ItemId = data.Targets[1].ItemId }).ToArray();
        Assert.Equal("AMBIGUOUS", HistoricalMetadataValidator.Preview(data.Input with { Rows = [first], SourceMap = conflict }, data.Targets, User()).Rows[0].Match);
        var foreign = new[] { data.Input.SourceMap[0] with { BusinessId = Guid.NewGuid() } };
        var row = HistoricalMetadataValidator.Preview(data.Input with { Rows = [first], SourceMap = foreign }, data.Targets, User()).Rows[0];
        Assert.Equal("OUT_OF_SCOPE", row.Match); Assert.Contains("SOURCE_TENANT_MISMATCH", row.Reasons); Assert.Null(row.Provenance); Assert.Empty(row.UnchangedCurrentValues);
    }
    [Fact]
    public void LegacyBlankUnitAndMissingGeometryRequireACorrectionContract()
    {
        var data = HistoricalPreviewFixtures.Create("valid", Business, Actor); var target = data.Targets[0] with { LineUnit = "", KgPerUnit = null };
        var row = HistoricalMetadataValidator.Preview(data.Input with { Rows = [data.Input.Rows[0]] }, [target], User()).Rows[0];
        Assert.Equal("UNIT_CORRECTION_REQUIRED", row.Fields.Single(x => x.Field == "historicalUnit").ReasonCode);
        Assert.Equal("GEOMETRY_CORRECTION_REQUIRED", row.Fields.Single(x => x.Field == "kgPerUnit").ReasonCode);
    }
}
