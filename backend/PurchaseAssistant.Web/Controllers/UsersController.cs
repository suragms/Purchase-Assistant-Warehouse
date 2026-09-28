using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Contracts.Responses;
using PurchaseAssistant.Domain.Constants;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ICurrentUserService _currentUser;
        // In a full implementation, we'd inject AppDbContext or a UserService here.

        public UsersController(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        [Authorize(Policy = "RequireUsersView")]
        public async Task<IActionResult> GetUsers()
        {
            // Placeholder: Fetch all memberships for _currentUser.BusinessId, map to DTOs
            return Ok(new ApiResponse<object>(new { }));
        }

        [HttpPost]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> CreateUser([FromBody] object request)
        {
            // Placeholder: Create user, membership, hash password, assign role
            return Ok(new ApiResponse<object>(new { }));
        }

        [HttpPost("{id}/block")]
        [Authorize(Policy = "RequireUsersManage")]
        public async Task<IActionResult> BlockUser(System.Guid id)
        {
            // Placeholder: Update UserStatus to Blocked
            return Ok(new ApiResponse<bool>(true));
        }
    }
}