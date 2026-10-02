using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PurchaseAssistant.Application.DTOs.Reports;
using PurchaseAssistant.Domain.Enums;

namespace PurchaseAssistant.UnitTests.AI;
public partial class PurchaseIntentEndpointTests
{
    [Theory, InlineData(Role.Owner), InlineData(Role.SuperAdmin)]
    public async Task HistoricalFixtureApiAllowsScopedPrivilegedMembershipOnlyAndReportsNoPersistence(Role role)
    {
        using var factory = new Factory { MemberRole = role }; using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var response = await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "mixed" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.Contains("no-store", response.Headers.CacheControl!.ToString());
        var result = await response.Content.ReadFromJsonAsync<HistoricalPreviewResult>(); Assert.NotNull(result);
        Assert.Equal(BusinessId, result.BusinessId); Assert.False(result.WritesPerformed); Assert.False(result.PersistenceAvailable); Assert.False(result.ConfirmationAvailable);
        Assert.True(result.Summary.TotalRows >= 30);
        factory.Purchases.VerifyNoOtherCalls(); factory.Reports.VerifyNoOtherCalls();
    }
    [Theory, InlineData(Role.Manager), InlineData(Role.Staff), InlineData(Role.Admin)]
    public async Task HistoricalFixtureApiDeniesOtherMembershipRolesEvenWithForgedReportPermission(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = "reports.view" }; using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "mixed" })).StatusCode);
    }
    [Theory, InlineData("unknown"), InlineData("real-upload")]
    public async Task HistoricalFixtureApiRejectsUnknownFixtures(string id)
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = id })).StatusCode);
    }
    [Fact]
    public async Task HistoricalFixtureApiRejectsTenantPayloadManipulationAndHasNoConfirmationRoute()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "valid", businessId = Guid.NewGuid(), rows = new[] { "real" } })).StatusCode);
        foreach (var action in new[] { "confirm", "commit", "apply", "import/execute" }) Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/v1/exports/historical/" + action, new { fixtureId = "valid" })).StatusCode);
        client.DefaultRequestHeaders.Authorization = null;
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "valid" })).StatusCode);
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", false));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/exports/historical/preview", new { fixtureId = "valid" })).StatusCode);
    }
}
