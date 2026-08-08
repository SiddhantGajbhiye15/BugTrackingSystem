using BugTrackingSystem.DTOs.Projects;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Repositories.Interfaces;
namespace BugTrackingSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;

        public ProjectService(
    IProjectRepository projectRepository,
    IProjectMemberRepository projectMemberRepository)
        {
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
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
                    normalizedProjectCode);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Project code already exists.");
            }

            var project = new Project
            {
                ProjectCode = normalizedProjectCode,
                ProjectName = request.ProjectName.Trim(),
                Description = request.Description.Trim(),
                Status = ProjectStatus.Active,

                // Original creator
                CreatedBy = currentUserId,

                // Current manager
                ProjectManagerId = currentUserId,

                CreatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            project = await _projectRepository.GetByIdAsync(
                project.ProjectId);

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

            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can update this project.");
            }

            project.ProjectName = request.ProjectName.Trim();
            project.Description = request.Description.Trim();
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync();

            return MapToResponseDto(project);
        }

        public async Task<ProjectResponseDto?> ChangeManagerAsync(
            int projectId,
            ChangeProjectManagerRequestDto request)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return null;
            }

            var newManager =
                await _projectRepository.GetUserByIdAsync(
                    request.ProjectManagerId);

            if (newManager == null)
            {
                throw new InvalidOperationException(
                    "Selected user was not found.");
            }

            if (!newManager.IsActive)
            {
                throw new InvalidOperationException(
                    "Inactive user cannot manage a project.");
            }

            if (newManager.Role != UserRole.ProjectManager)
            {
                throw new InvalidOperationException(
                    "Selected user must be a Project Manager.");
            }

            if (project.ProjectManagerId == newManager.UserId)
            {
                throw new InvalidOperationException(
                    "This user is already managing the project.");
            }

            project.ProjectManagerId = newManager.UserId;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync();

            project = await _projectRepository.GetByIdAsync(
                projectId);

            return MapToResponseDto(project!);
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

            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can delete this project.");
            }

            var hasMembersOrBugs =
                await _projectRepository.HasMembersOrBugsAsync(
                    projectId);

            if (hasMembersOrBugs)
            {
                throw new InvalidOperationException(
                    "A project containing members or bugs cannot be deleted.");
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

                ProjectManagerId = project.ProjectManagerId,

                ProjectManagerName =
                    project.ProjectManager == null
                        ? "Not Assigned"
                        : $"{project.ProjectManager.FirstName} " +
                          $"{project.ProjectManager.LastName}",

                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }
        public async Task<ProjectResponseDto> CompleteAsync(
    int projectId,
    int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new KeyNotFoundException(
                    "Project not found.");
            }

            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can complete this project.");
            }

            if (project.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "Only an active project can be completed.");
            }

            project.Status = ProjectStatus.Completed;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync();

            return MapToResponseDto(project);
        }
        public async Task<ProjectResponseDto> ArchiveAsync(
    int projectId,
    int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new KeyNotFoundException(
                    "Project not found.");
            }

            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can archive this project.");
            }

            if (project.Status != ProjectStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Only a completed project can be archived.");
            }

            var activeMembers =
                await _projectMemberRepository
                    .GetActiveMembersForUpdateAsync(projectId);

            var archivedAt = DateTime.UtcNow;

            foreach (var member in activeMembers)
            {
                member.RemovedDate = archivedAt;
            }

            project.Status = ProjectStatus.Archived;
            project.UpdatedAt = archivedAt;

            await _projectMemberRepository.SaveChangesAsync();
            await _projectRepository.SaveChangesAsync();

            return MapToResponseDto(project);
        }
        public async Task<ProjectResponseDto> RestoreAsync(
    int projectId,
    int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                throw new KeyNotFoundException(
                    "Project not found.");
            }

            if (project.ProjectManagerId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the current Project Manager can restore this project.");
            }

            if (project.Status != ProjectStatus.Archived)
            {
                throw new InvalidOperationException(
                    "Only an archived project can be restored.");
            }

            project.Status = ProjectStatus.Active;
            project.UpdatedAt = DateTime.UtcNow;

            await _projectRepository.SaveChangesAsync();

            return MapToResponseDto(project);
        }

    }
}