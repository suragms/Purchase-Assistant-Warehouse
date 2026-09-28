using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using PurchaseAssistant.Web.Authorization;
using System.Security.Claims;

namespace PurchaseAssistant.UnitTests.Auth;

public class PermissionAuthorizationHandlerTests
{
    private readonly PermissionAuthorizationHandler _sut;

    public PermissionAuthorizationHandlerTests()
    {
        _sut = new PermissionAuthorizationHandler();
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenUserHasExplicitPermission()
    {
        // Arrange
        var requirement = new PermissionRequirement("catalog.view");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("permissions", "catalog.view"),
            new Claim("permissions", "catalog.edit")
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenUserIsOwner()
    {
        // Arrange
        var requirement = new PermissionRequirement("catalog.view");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "Owner")
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenUserIsSuperAdmin()
    {
        // Arrange
        var requirement = new PermissionRequirement("settings.manage");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "SuperAdmin")
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldNotSucceed_WhenUserLacksPermission()
    {
        // Arrange
        var requirement = new PermissionRequirement("catalog.edit");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("role", "Manager"),
            new Claim("permissions", "catalog.view") // missing edit
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }
}