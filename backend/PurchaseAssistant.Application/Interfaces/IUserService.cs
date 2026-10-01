using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseAssistant.Application.DTOs.Users;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserPermissionsDto?> GetPermissionsAsync(Guid userId);
        Task<bool> PatchPermissionsAsync(Guid userId, PatchPermissionsDto patch);
    }
}
