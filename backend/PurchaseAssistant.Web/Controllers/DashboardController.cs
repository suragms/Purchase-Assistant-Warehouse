using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseAssistant.Application.Interfaces;
using System.Threading.Tasks;

namespace PurchaseAssistant.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var data = await _dashboardService.GetDashboardDataAsync();
            return Ok(data);
        }
    }
}
