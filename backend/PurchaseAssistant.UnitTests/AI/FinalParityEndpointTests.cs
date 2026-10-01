using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Constants;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SkiaSharp;
namespace PurchaseAssistant.UnitTests.AI;
public partial class PurchaseIntentEndpointTests
{
    [Theory]
    [InlineData(Role.Owner)] [InlineData(Role.Admin)]
    public async Task AuthorizedAdministratorCreatesStaffWhoCanLoginRefreshAndLogout(Role actor)
    {
        using var factory = new Factory { MemberRole = actor, Permission = Permissions.UsersManage };
        using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("users.manage", true));
        var create = await client.PostAsJsonAsync("/api/v1/users", new { name = "Warehouse Staff", email = "  STAFF@example.test ", password = "test-staff-password", role = 4 });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        using var staff = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        var login = await staff.PostAsJsonAsync("/api/v1/auth/login", new { email = "staff@example.test", password = "test-staff-password" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var payload = JsonDocument.Parse(await login.Content.ReadAsStringAsync()).RootElement.GetProperty("data");
        Assert.Equal("Staff", payload.GetProperty("user").GetProperty("currentBusiness").GetProperty("role").GetString());
        Assert.DoesNotContain("users.manage", payload.GetProperty("user").GetProperty("currentBusiness").GetProperty("permissions").ToString());
        staff.DefaultRequestHeaders.Authorization = new("Bearer", payload.GetProperty("accessToken").GetString());
        Assert.Equal(HttpStatusCode.OK, (await staff.GetAsync("/api/v1/auth/me")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await staff.GetAsync("/api/v1/users")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await staff.GetAsync("/api/v1/exports/stock.xlsx")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await staff.PostAsJsonAsync("/api/v1/auth/select-business", new { businessId = Guid.NewGuid() })).StatusCode);
        var refresh = await staff.PostAsJsonAsync("/api/v1/auth/refresh", new {}); Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);
        var fresh = JsonDocument.Parse(await refresh.Content.ReadAsStringAsync()).RootElement.GetProperty("data").GetProperty("accessToken").GetString();
        staff.DefaultRequestHeaders.Authorization = new("Bearer", fresh);
        Assert.True((await staff.PostAsJsonAsync("/api/v1/auth/logout-all", new {})).IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await staff.GetAsync("/api/v1/auth/me")).StatusCode);
    }
    [Theory]
    [InlineData(Role.Owner)] [InlineData(Role.Manager)] [InlineData(Role.Staff)]
    public async Task EachRoleCanLoginAndWrongPasswordsFail(Role role)
    {
        using var factory = new Factory { MemberRole = role };
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var u = await db.Users.SingleAsync(x => x.Id == UserId);
            u.PasswordHash = scope.ServiceProvider.GetRequiredService<IPasswordHasher>().HashPassword("correct-password"); await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "endpoint@test.local", password = "wrong-password" })).StatusCode);
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "endpoint@test.local", password = "correct-password" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains($"\"role\":\"{role}\"", await response.Content.ReadAsStringAsync());
    }
    [Theory]
    [InlineData(UserStatus.Inactive)] [InlineData(UserStatus.Blocked)] [InlineData(UserStatus.Deleted)]
    public async Task NonActiveStaffCannotLogin(UserStatus status)
    {
        using var factory = new Factory { MemberRole = Role.Staff }; using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var u = await db.Users.SingleAsync(x => x.Id == UserId);
            u.PasswordHash = scope.ServiceProvider.GetRequiredService<IPasswordHasher>().HashPassword("correct-password"); u.Status = status; await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "endpoint@test.local", password = "correct-password" })).StatusCode);
    }
    [Theory]
    [InlineData(Role.Manager)] [InlineData(Role.Staff)]
    public async Task UserManagementRoleGateCannotBeBypassedByPermissionClaim(Role role)
    {
        using var factory = new Factory { MemberRole = role, Permission = Permissions.UsersManage }; using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(Permissions.UsersManage, true));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/users", new { name = "Staff", email = "staff@test.local", password = "valid-password", role = 4 })).StatusCode);
    }
    [Theory]
    [InlineData("", "staff@test.local", "valid-password", 4)]
    [InlineData("Staff", "not-email", "valid-password", 4)]
    [InlineData("Staff", "staff@test.local", "short", 4)]
    [InlineData("Staff", "staff@test.local", "valid-password", 99)]
    [InlineData("Staff", "staff@test.local", "valid-password", 0)]
    [InlineData("Staff", "staff@test.local", "valid-password", 1)]
    public async Task InvalidUserCreationIsRejected(string name, string email, string password, int role)
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("users.manage", true));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/v1/users", new { name, email, password, role })).StatusCode);
    }
    [Fact]
    public async Task DuplicateEmailDoesNotModifyExistingMembership()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token("users.manage", true));
        var response = await client.PostAsJsonAsync("/api/v1/users", new { name = "Replacement", email = "ENDPOINT@test.local", password = "valid-password", role = 4 });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(Role.Owner, (await db.Memberships.SingleAsync(x => x.UserId == UserId)).Role);
    }
    [Fact]
    public async Task StaffChecklistIsPrivateAndCompletionIsIdempotent()
    {
        using var factory = new Factory(); using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        var path = "/api/v1/operations/checklist/0/open_check";
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsJsonAsync(path, new {})).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsJsonAsync(path, new {})).StatusCode);
        var response = await client.GetAsync("/api/v1/operations/checklist/today"); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("completedTasks").GetInt32());
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PutAsJsonAsync("/api/v1/operations/checklist/templates", new object[] {})).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/operations/usage", new { lines = new object[] {} })).StatusCode);
        using var scope = factory.Services.CreateScope(); Assert.Single(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Set<ChecklistCompletion>().IgnoreQueryFilters().ToListAsync());
    }
    [Fact]
    public async Task SettingsRespectOwnerAndBusinessBoundaries()
    {
        using var factory = new Factory { MemberRole = Role.Staff }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("stock.view", true));
        var foreignId = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.Businesses.Add(new Business { Id = foreignId, Name = "SECRET OTHER BUSINESS" }); await db.SaveChangesAsync(); }
        var profile = await client.GetAsync("/api/v1/settings/business"); Assert.Equal(HttpStatusCode.OK, profile.StatusCode); Assert.DoesNotContain("SECRET OTHER", await profile.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PutAsJsonAsync("/api/v1/settings/business", new { name = "Hacked" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/v1/settings/business/logo")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/v1/settings/notifications", new { notificationsEnabled = false, notificationKinds = new[] { "delivery" } })).StatusCode);
        Assert.Contains("false", await client.GetStringAsync("/api/v1/settings/notifications"));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PutAsJsonAsync("/api/v1/settings/notifications", new { notificationKinds = new[] { "made-up" } })).StatusCode);
    }
    [Fact]
    public void LogoValidationDecodesAndRejectsSpoofedMalformedAndOversizedFiles()
    {
        using var bitmap = new SKBitmap(2, 2); bitmap.Erase(SKColors.Blue); using var image = SKImage.FromBitmap(bitmap); using var png = image.Encode(SKEncodedImageFormat.Png, 100); var bytes = png.ToArray();
        Assert.NotEmpty(LogoImageValidator.ValidateAndNormalize(bytes, "image/png", "logo.png"));
        Assert.Throws<ArgumentException>(() => LogoImageValidator.ValidateAndNormalize(bytes, "image/jpeg", "logo.jpg"));
        Assert.Throws<ArgumentException>(() => LogoImageValidator.ValidateAndNormalize(bytes, "image/png", "../logo.png"));
        Assert.Throws<ArgumentException>(() => LogoImageValidator.ValidateAndNormalize(bytes, "image/png", "logo.exe"));
        Assert.Throws<ArgumentException>(() => LogoImageValidator.ValidateAndNormalize([1,2,3], "image/png", "logo.png"));
        Assert.Throws<ArgumentException>(() => LogoImageValidator.ValidateAndNormalize(new byte[LogoImageValidator.MaxBytes+1], "image/png", "logo.png"));
    }
    [Fact]
    public async Task ExportFilesAreValidAndContainNoAccountSecrets()
    {
        using var factory = new Factory { MemberRole = Role.Owner }; using var client = factory.CreateClient(); client.DefaultRequestHeaders.Authorization = new("Bearer", Token("reports.view", true));
        var response = await client.GetAsync("/api/v1/exports/backup.json"); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync(); Assert.DoesNotContain("Password", text); Assert.DoesNotContain("Token", text); Assert.Contains("businessId", text);
        var xlsx = await client.GetByteArrayAsync("/api/v1/exports/stock.xlsx"); using var zip = new System.IO.Compression.ZipArchive(new MemoryStream(xlsx)); Assert.NotNull(zip.GetEntry("xl/worksheets/sheet1.xml"));
        var pdf = await client.GetByteArrayAsync("/api/v1/exports/purchases.pdf"); Assert.StartsWith("%PDF", System.Text.Encoding.ASCII.GetString(pdf));
    }
}
