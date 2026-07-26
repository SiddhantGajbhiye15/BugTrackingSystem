using BugTrackingSystem.DTOs.ProjectMembers;
using BugTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:int}")]
    [Authorize(Roles = "ProjectManager")]
    public class ProjectMembersController : ControllerBase
    {
        private readonly IProjectMemberService _projectMemberService;

        public ProjectMembersController(
            IProjectMemberService projectMemberService)
        {
            _projectMemberService = projectMemberService;
        }

        // GET: /api/projects/1/members
        [HttpGet("members")]
        public async Task<IActionResult> GetProjectMembers(
            int projectId)
        {
            try
            {
                int currentUserId = GetCurrentUserId();

                var members =
                    await _projectMemberService
                        .GetProjectMembersAsync(
                            projectId,
                            currentUserId);

                return Ok(members);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        // GET: /api/projects/1/members/available-users
        [HttpGet("members/available-users")]
        public async Task<IActionResult> GetAvailableUsers(
            int projectId)
        {
            try
            {
                int currentUserId = GetCurrentUserId();

                var users =
                    await _projectMemberService
                        .GetAvailableUsersAsync(
                            projectId,
                            currentUserId);

                return Ok(users);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // POST: /api/projects/1/members
        [HttpPost("members")]
        public async Task<IActionResult> AddProjectMember(
            int projectId,
            [FromBody] AddProjectMemberRequestDto request)
        {
            try
            {
                int currentUserId = GetCurrentUserId();

                var member =
                    await _projectMemberService
                        .AddProjectMemberAsync(
                            projectId,
                            request,
                            currentUserId);

                return StatusCode(
                    StatusCodes.Status201Created,
                    member);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: /api/projects/1/members/5
        [HttpDelete("members/{projectMemberId:int}")]
        public async Task<IActionResult> RemoveProjectMember(
            int projectId,
            int projectMemberId)
        {
            try
            {
                int currentUserId = GetCurrentUserId();

                await _projectMemberService
                    .RemoveProjectMemberAsync(
                        projectId,
                        projectMemberId,
                        currentUserId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            string? userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("userId");

            if (!int.TryParse(userIdValue, out int userId))
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user ID is invalid.");
            }

            return userId;
        }
    }
}