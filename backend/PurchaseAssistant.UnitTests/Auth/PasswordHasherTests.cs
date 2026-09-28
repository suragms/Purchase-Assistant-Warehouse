using FluentAssertions;
using PurchaseAssistant.Infrastructure.Auth;

namespace PurchaseAssistant.UnitTests.Auth;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut;

    public PasswordHasherTests()
    {
        _sut = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedString_NotEqualToOriginal()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash = _sut.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_ForValidHashAndOriginalPassword()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForInvalidPassword()
    {
        // Arrange
        var password = "SecurePassword123!";
        var invalidPassword = "WrongPassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(invalidPassword, hash);

        // Assert
        result.Should().BeFalse();
    }
}