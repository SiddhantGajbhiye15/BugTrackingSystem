using BugTrackingSystem.DTOs.Dashboard;
using BugTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminDashboardResponseDto>>
            GetAdminDashboard()
        {
            var dashboard =
                await _dashboardService.GetAdminDashboardAsync();

            return Ok(dashboard);
        }
    }
}