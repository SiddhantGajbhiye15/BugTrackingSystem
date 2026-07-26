using BugTrackingSystem.DTOs.Bugs;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class BugsController : ControllerBase
    {
        private readonly IBugService _bugService;

        public BugsController(IBugService bugService)
        {
            _bugService = bugService;
        }

        // GET: /api/projects/1/bugs
        [HttpGet("projects/{projectId:int}/bugs")]
        public async Task<IActionResult> GetProjectBugs(
            int projectId,
            [FromQuery] BugStatus? status,
            [FromQuery] BugPriority? priority,
            [FromQuery] int? assignedDeveloperId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bugs = await _bugService.GetProjectBugsAsync(
                    projectId,
                    status,
                    priority,
                    assignedDeveloperId,
                    currentUserId,
                    currentUserRole);

                return Ok(bugs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // GET: /api/bugs/my-assigned
        [HttpGet("bugs/my-assigned")]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> GetMyAssignedBugs()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bugs = await _bugService.GetMyAssignedBugsAsync(
                    currentUserId,
                    currentUserRole);

                return Ok(bugs);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // GET: /api/bugs/1
        [HttpGet("bugs/{bugId:int}")]
        public async Task<IActionResult> GetBugById(int bugId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.GetBugByIdAsync(
                    bugId,
                    currentUserId,
                    currentUserRole);

                return Ok(bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // POST: /api/projects/1/bugs
        [HttpPost("projects/{projectId:int}/bugs")]
        [Authorize(Roles = "Tester")]
        public async Task<IActionResult> CreateBug(
            int projectId,
            [FromBody] CreateBugRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.CreateBugAsync(
                    projectId,
                    request,
                    currentUserId,
                    currentUserRole);

                return StatusCode(
                    StatusCodes.Status201Created,
                    bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: /api/bugs/1
        [HttpPut("bugs/{bugId:int}")]
        [Authorize(Roles = "Tester")]
        public async Task<IActionResult> UpdateBug(
            int bugId,
            [FromBody] UpdateBugRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.UpdateBugAsync(
                    bugId,
                    request,
                    currentUserId,
                    currentUserRole);

                return Ok(bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: /api/bugs/1/assign
        [HttpPatch("bugs/{bugId:int}/assign")]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> AssignBug(
            int bugId,
            [FromBody] AssignBugRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.AssignBugAsync(
                    bugId,
                    request,
                    currentUserId,
                    currentUserRole);

                return Ok(bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: /api/bugs/1/status
        [HttpPatch("bugs/{bugId:int}/status")]
        [Authorize(Roles = "Developer,Tester")]
        public async Task<IActionResult> ChangeBugStatus(
            int bugId,
            [FromBody] ChangeBugStatusRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.ChangeBugStatusAsync(
                    bugId,
                    request,
                    currentUserId,
                    currentUserRole);

                return Ok(bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: /api/bugs/1/priority
        [HttpPatch("bugs/{bugId:int}/priority")]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> ChangeBugPriority(
            int bugId,
            [FromBody] ChangeBugPriorityRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var bug = await _bugService.ChangeBugPriorityAsync(
                    bugId,
                    request,
                    currentUserId,
                    currentUserRole);

                return Ok(bug);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: /api/bugs/1
        [HttpDelete("bugs/{bugId:int}")]
        [Authorize(Roles = "ProjectManager,Tester")]
        public async Task<IActionResult> DeleteBug(int bugId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                await _bugService.DeleteBugAsync(
                    bugId,
                    currentUserId,
                    currentUserRole);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("userId");

            if (!int.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated user ID is invalid.");
            }

            return userId;
        }

        private UserRole GetCurrentUserRole()
        {
            var roleValue =
                User.FindFirstValue(ClaimTypes.Role)
                ?? User.FindFirstValue("role");

            if (!Enum.TryParse<UserRole>(
                    roleValue,
                    true,
                    out var role))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated user role is invalid.");
            }

            return role;
        }
    }
}