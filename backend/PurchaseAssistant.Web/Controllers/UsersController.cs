using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.DTOs.Users;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Contracts.Responses;
using System;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUser;

        public UsersController(IUserService userService, ICurrentUserService currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [Authorize(Policy = "RequireUsersView")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse<object>(users));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireUsersView")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>(null, "User not found."));
            }
            return Ok(new ApiResponse<object>(user));
        }

        [HttpPost]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, new ApiResponse<object>(created));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto request)
        {
            var success = await _userService.UpdateUserAsync(id, request);
            if (!success)
            {
                return NotFound(new ApiResponse<object>(null, "User not found."));
            }
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpPost("{id}/block")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>(null, "User not found."));
            }

            var updateDto = new UpdateUserDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Status = Domain.Enums.UserStatus.Blocked
            };

            await _userService.UpdateUserAsync(id, updateDto);
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>(null, "User not found."));
            }
            return Ok(new ApiResponse<bool>(true));
        }

        [HttpGet("{id}/permissions")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> GetUserPermissions(Guid id)
        {
            var perms = await _userService.GetPermissionsAsync(id);
            if (perms == null)
                return NotFound(new ApiResponse<object>(null, "User not found."));
            return Ok(new ApiResponse<object>(perms));
        }

        [HttpPatch("{id}/permissions")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> PatchUserPermissions(Guid id, [FromBody] Application.DTOs.Users.PatchPermissionsDto patch)
        {
            var success = await _userService.PatchPermissionsAsync(id, patch);
            if (!success)
                return NotFound(new ApiResponse<object>(null, "User not found."));
            return Ok(new ApiResponse<bool>(true));
        }
    }
}
