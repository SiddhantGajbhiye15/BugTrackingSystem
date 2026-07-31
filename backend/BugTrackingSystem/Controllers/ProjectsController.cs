using BugTrackingSystem.DTOs.Projects;
using BugTrackingSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(
            IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateProject(
            CreateProjectRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var project =
                    await _projectService.CreateAsync(
                        request,
                        currentUserId);

                return CreatedAtAction(
                    nameof(GetProjectById),
                    new { id = project.ProjectId },
                    project);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects =
                await _projectService.GetAllAsync();

            // Admin can view every project.
            if (User.IsInRole("Admin"))
            {
                return Ok(projects);
            }

            // A Project Manager can view only projects currently managed by them.
            var currentUserId = GetCurrentUserId();

            var managedProjects = projects
                .Where(project =>
                    project.ProjectManagerId == currentUserId)
                .ToList();

            return Ok(managedProjects);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project =
                await _projectService.GetByIdAsync(id);

            if (project == null)
            {
                return NotFound(new
                {
                    Message = "Project not found."
                });
            }

            if (
                User.IsInRole("ProjectManager") &&
                project.ProjectManagerId != GetCurrentUserId()
            )
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        Message =
                            "You are not authorized to view this project."
                    });
            }

            return Ok(project);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> UpdateProject(
            int id,
            UpdateProjectRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var project =
                    await _projectService.UpdateAsync(
                        id,
                        request,
                        currentUserId);

                if (project == null)
                {
                    return NotFound(new
                    {
                        Message = "Project not found."
                    });
                }

                return Ok(project);
            }
            catch (UnauthorizedAccessException exception)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        Message = exception.Message
                    });
            }
        }

        [HttpPatch("{id:int}/manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeProjectManager(
            int id,
            ChangeProjectManagerRequestDto request)
        {
            try
            {
                var project =
                    await _projectService.ChangeManagerAsync(
                        id,
                        request);

                if (project == null)
                {
                    return NotFound(new
                    {
                        Message = "Project not found."
                    });
                }

                return Ok(project);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var deleted =
                    await _projectService.DeleteAsync(
                        id,
                        currentUserId);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        Message = "Project not found."
                    });
                }

                return Ok(new
                {
                    Message = "Project deleted successfully."
                });
            }
            catch (UnauthorizedAccessException exception)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        Message = exception.Message
                    });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            return int.Parse(userIdValue!);
        }
    }
}