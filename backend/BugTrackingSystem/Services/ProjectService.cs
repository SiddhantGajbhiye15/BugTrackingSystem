using BugTrackingSystem.DTOs.Projects;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Interfaces;

namespace BugTrackingSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(
            IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectResponseDto> CreateAsync(
            CreateProjectRequestDto request,
            int currentUserId)
        {
            var normalizedProjectCode = request.ProjectCode
                .Trim()
                .ToUpperInvariant();

            var codeExists =
                await _projectRepository.ProjectCodeExistsAsync(
                    normalizedProjectCode
                );

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Project code already exists."
                );
            }

            var project = new Project
            {
                ProjectCode = normalizedProjectCode,
                ProjectName = request.ProjectName.Trim(),
                Description = request.Description.Trim(),
                Status = ProjectStatus.Active,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            project = await _projectRepository.GetByIdAsync(
                project.ProjectId
            );

            return MapToResponseDto(project!);
        }

        public async Task<List<ProjectResponseDto>> GetAllAsync()
        {
            var projects =
                await _projectRepository.GetAllAsync();

            return projects
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<ProjectResponseDto?> GetByIdAsync(
            int projectId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return null;
            }

            return MapToResponseDto(project);
        }

        public async Task<ProjectResponseDto?> UpdateAsync(
            int projectId,
            UpdateProjectRequestDto request,
            int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return null;
            }

            if (project.CreatedBy != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can update only projects created by you."
                );
            }

            project.ProjectName = request.ProjectName.Trim();
            project.Description = request.Description.Trim();
            project.Status = request.Status;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync();

            return MapToResponseDto(project);
        }

        public async Task<bool> DeleteAsync(
            int projectId,
            int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return false;
            }

            if (project.CreatedBy != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can delete only projects created by you."
                );
            }

            var hasMembersOrBugs =
                await _projectRepository.HasMembersOrBugsAsync(
                    projectId
                );

            if (hasMembersOrBugs)
            {
                throw new InvalidOperationException(
                    "A project containing members or bugs cannot be deleted."
                );
            }

            _projectRepository.Delete(project);
            await _projectRepository.SaveChangesAsync();

            return true;
        }

        private static ProjectResponseDto MapToResponseDto(
            Project project)
        {
            return new ProjectResponseDto
            {
                ProjectId = project.ProjectId,
                ProjectCode = project.ProjectCode,
                ProjectName = project.ProjectName,
                Description = project.Description,
                Status = project.Status,
                CreatedBy = project.CreatedBy,
                CreatedByName =
                    $"{project.CreatedByUser.FirstName} " +
                    $"{project.CreatedByUser.LastName}",
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }
    }
}