using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PurchaseAssistant.Application.DTOs.Users;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Infrastructure.Data;
using PurchaseAssistant.Domain.Entities;
using BCrypt.Net;

namespace PurchaseAssistant.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UserService(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var businessId = _currentUser.BusinessId ?? Guid.Empty;
            var memberships = await _context.Memberships
                .AsNoTracking()
                .Where(m => m.BusinessId == businessId)
                .Include(m => m.User)
                .ToListAsync();

            return memberships.Select(m => new UserDto
            {
                Id = m.User.Id,
                Name = m.User.Name,
                Email = m.User.Email,
                Status = m.User.Status,
                Role = m.Role,
                CreatedAt = m.User.CreatedAt
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var businessId = _currentUser.BusinessId ?? Guid.Empty;
            var membership = await _context.Memberships
                .AsNoTracking()
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.BusinessId == businessId && m.UserId == id);

            if (membership == null) return null;

            return new UserDto
            {
                Id = membership.User.Id,
                Name = membership.User.Name,
                Email = membership.User.Email,
                Status = membership.User.Status,
                Role = membership.Role,
                CreatedAt = membership.User.CreatedAt
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var businessId = _currentUser.BusinessId ?? Guid.Empty;

            // Check if user already exists globally by email
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            User user;

            if (existingUser != null)
            {
                user = existingUser;
            }
            else
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = createUserDto.Name,
                    Email = createUserDto.Email,
                    PasswordHash = passwordHash,
                    Status = Domain.Enums.UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
            }

            // Check if membership already exists for this business
            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.BusinessId == businessId && m.UserId == user.Id);

            if (existingMembership == null)
            {
                var membership = new Membership
                {
                    Id = Guid.NewGuid(),
                    BusinessId = businessId,
                    UserId = user.Id,
                    Role = createUserDto.Role
                };
                _context.Memberships.Add(membership);
            }
            else
            {
                existingMembership.Role = createUserDto.Role;
            }

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Status = user.Status,
                Role = createUserDto.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var businessId = _currentUser.BusinessId ?? Guid.Empty;
            var membership = await _context.Memberships
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.BusinessId == businessId && m.UserId == id);

            if (membership == null) return false;

            membership.User.Name = updateUserDto.Name;
            membership.User.Email = updateUserDto.Email;
            membership.User.Status = updateUserDto.Status;
            membership.Role = updateUserDto.Role;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var businessId = _currentUser.BusinessId ?? Guid.Empty;
            var membership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.BusinessId == businessId && m.UserId == id);

            if (membership == null) return false;

            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
