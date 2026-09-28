using FluentAssertions;
using Microsoft.Extensions.Options;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace PurchaseAssistant.UnitTests.Auth;

public class JwtProviderTests
{
    private readonly JwtOptions _jwtOptions;
    private readonly JwtProvider _sut;

    public JwtProviderTests()
    {
        _jwtOptions = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "SuperSecretKeyForDevelopmentOnlyMakeSureToChangeInProduction12345!",
            ExpirationMinutes = 15
        };

        _sut = new JwtProvider(Options.Create(_jwtOptions));
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeStandardClaims_WhenNoMembership()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "John Doe"
        };

        // Act
        var token = _sut.GenerateAccessToken(user, null);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Audience);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().NotContain(c => c.Type == "businessId");
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeBusinessClaims_WhenMembershipProvided()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "member@example.com",
            Name = "Jane Doe"
        };

        var businessId = Guid.NewGuid();
        var membership = new Membership
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BusinessId = businessId,
            Role = Role.Manager,
            PermissionsJson = "[\"catalog.view\",\"catalog.edit\"]"
        };

        // Act
        var token = _sut.GenerateAccessToken(user, membership);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == "businessId" && c.Value == businessId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == Role.Manager.ToString());

        var perms = jwtToken.Claims.Where(c => c.Type == "permissions").Select(c => c.Value).ToList();
        perms.Should().Contain("catalog.view");
        perms.Should().Contain("catalog.edit");
    }

    [Fact]
    public void GenerateRandomToken_ShouldReturnBase64String()
    {
        // Act
        var token = _sut.GenerateRandomToken();

        // Assert
        token.Should().NotBeNullOrEmpty();
        // Check if it's base64 (Convert.FromBase64String shouldn't throw format exception)
        Action act = () => Convert.FromBase64String(token);
        act.Should().NotThrow();
    }
}