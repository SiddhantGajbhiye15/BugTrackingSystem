using System.Security.Claims;
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

        // GET: /api/dashboard/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminDashboardResponseDto>>
            GetAdminDashboard()
        {
            var dashboard =
                await _dashboardService.GetAdminDashboardAsync();

            return Ok(dashboard);
        }

        // GET: /api/dashboard/project-manager
        [HttpGet("project-manager")]
        [Authorize(Roles = "ProjectManager")]
        public async Task<
            ActionResult<ProjectManagerDashboardResponseDto>>
            GetProjectManagerDashboard()
        {
            int currentUserId = GetCurrentUserId();

            var dashboard = await _dashboardService
                .GetProjectManagerDashboardAsync(currentUserId);

            return Ok(dashboard);
        }

        private int GetCurrentUserId()
        {
            string? userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                userIdClaim,
                out int currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return currentUserId;
        }
        // GET: /api/dashboard/tester
        [HttpGet("tester")]
        [Authorize(Roles = "Tester")]
        public async Task<ActionResult<TesterDashboardResponseDto>>
            GetTesterDashboard()
        {
            int currentUserId = GetCurrentUserId();

            var dashboard = await _dashboardService
                .GetTesterDashboardAsync(currentUserId);

            return Ok(dashboard);
        }
        // GET: /api/dashboard/developer
        [HttpGet("developer")]
        [Authorize(Roles = "Developer")]
        public async Task<ActionResult<DeveloperDashboardResponseDto>>
            GetDeveloperDashboard()
        {
            int currentUserId = GetCurrentUserId();

            var dashboard = await _dashboardService
                .GetDeveloperDashboardAsync(currentUserId);

            return Ok(dashboard);
        }
    }
}