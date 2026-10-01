using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Users;
using PurchaseAssistant.Domain.Entities;
using PurchaseAssistant.Domain.Enums;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Infrastructure.Services;
using PurchaseAssistant.Application.Interfaces;
using Xunit;

namespace PurchaseAssistant.UnitTests.Services;

public class StubCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public string Email => "test@test.com";
    public Guid? BusinessId { get; set; }
    public string Role { get; set; } = "Staff";
    public System.Collections.Generic.IEnumerable<string> Permissions { get; set; } = new List<string>();
    public bool HasPermission(string permission) => Permissions.Contains(permission);
}

public class UserServiceRBACStaffTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly StubCurrentUserService _currentUser;
    private readonly UserService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _ownerUserId = Guid.NewGuid();
    private readonly Guid _managerUserId = Guid.NewGuid();

    public UserServiceRBACStaffTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var stubTenant = new StubTenantProvider { BusinessId = _businessId };

        _context = new AppDbContext(options, stubTenant);

        _context.Businesses.Add(new Business { Id = _businessId, Name = "Test Business", IsActive = true });
        _context.Users.Add(new User { Id = _ownerUserId, Name = "Owner", Email = "owner@test.com", PasswordHash = "hash" });
        _context.Users.Add(new User { Id = _managerUserId, Name = "Manager", Email = "manager@test.com", PasswordHash = "hash" });

        _context.Memberships.Add(new Membership { BusinessId = _businessId, UserId = _ownerUserId, Role = Role.Owner });
        _context.Memberships.Add(new Membership { BusinessId = _businessId, UserId = _managerUserId, Role = Role.Manager });

        _context.SaveChanges();

        _currentUser = new StubCurrentUserService { BusinessId = _businessId };
        _sut = new UserService(_context, _currentUser);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Manager_CreatesStaffUser_Allowed()
    {
        // Arrange
        _currentUser.UserId = _managerUserId;
        _currentUser.Role = "Manager";
        _currentUser.Permissions = new List<string> { "users.manage" };

        var newUser = new CreateUserDto
        {
            Name = "New Staff",
            Email = "staff@test.com",
            Password = "Password123!",
            Role = Role.Staff
        };

        // Act
        var result = await _sut.CreateUserAsync(newUser);

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be(Role.Staff);
    }

    [Fact]
    public async Task Manager_AssignsPrivilegedRole_Rejected()
    {
        // Arrange
        _currentUser.UserId = _managerUserId;
        _currentUser.Role = "Manager";
        _currentUser.Permissions = new List<string> { "users.manage" };

        var newUser = new CreateUserDto
        {
            Name = "New Admin",
            Email = "admin@test.com",
            Password = "Password123!",
            Role = Role.Admin
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.CreateUserAsync(newUser));
    }

    [Fact]
    public async Task Staff_CreatesStaffUser_Rejected()
    {
        // Arrange
        _currentUser.UserId = Guid.NewGuid();
        _currentUser.Role = "Staff";
        _currentUser.Permissions = new List<string> { };

        var newUser = new CreateUserDto
        {
            Name = "Another Staff",
            Email = "another@test.com",
            Password = "Password123!",
            Role = Role.Staff
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.CreateUserAsync(newUser));
    }
}
