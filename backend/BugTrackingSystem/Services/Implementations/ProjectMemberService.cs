using BugTrackingSystem.DTOs.ProjectMembers;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Repositories.Interfaces;
using BugTrackingSystem.Services.Interfaces;

namespace BugTrackingSystem.Services.Implementations
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectMemberService(
            IProjectMemberRepository projectMemberRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _projectMemberRepository = projectMemberRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ProjectMemberResponseDto>>
            GetProjectMembersAsync(
                int projectId,
                int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            ValidateProjectAndManager(project, currentUserId);

            var members =
                await _projectMemberRepository
                    .GetActiveMembersByProjectIdAsync(projectId);

            return members
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<List<AvailableUserResponseDto>>
            GetAvailableUsersAsync(
                int projectId,
                int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            ValidateProjectAndManager(project, currentUserId);

            if (project!.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "Members can only be assigned to an active project.");
            }

            var users =
                await _projectMemberRepository
                    .GetAvailableUsersAsync();

            return users
                .Select(user => new AvailableUserResponseDto
                {
                    UserId = user.UserId,
                    FullName =
                        $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Role = user.Role.ToString()
                })
                .ToList();
        }

        public async Task<ProjectMemberResponseDto>
            AddProjectMemberAsync(
                int projectId,
                AddProjectMemberRequestDto request,
                int currentUserId)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException(
                    "A valid user ID is required.");
            }

            var project =
                await _projectRepository.GetByIdAsync(projectId);

            ValidateProjectAndManager(project, currentUserId);

            if (project!.Status != ProjectStatus.Active)
            {
                throw new InvalidOperationException(
                    "Members can only be added to an active project.");
            }

            var user =
                await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "An inactive user cannot be added to a project.");
            }

            if (user.Role != UserRole.Developer &&
                user.Role != UserRole.Tester)
            {
                throw new InvalidOperationException(
                    "Only Developers and Testers can be added as project members.");
            }

            var activeMembership =
                await _projectMemberRepository
                    .GetActiveMembershipByUserIdAsync(request.UserId);

            if (activeMembership != null)
            {
                if (activeMembership.ProjectId == projectId)
                {
                    throw new InvalidOperationException(
                        "The user is already an active member of this project.");
                }

                throw new InvalidOperationException(
                    "The user is already assigned to another active project.");
            }

            var previousMembership =
                await _projectMemberRepository
                    .GetMembershipAsync(
                        projectId,
                        request.UserId);

            if (previousMembership != null)
            {
                previousMembership.JoinedDate = DateTime.UtcNow;
                previousMembership.RemovedDate = null;

                await _projectMemberRepository.SaveChangesAsync();

                return MapToResponseDto(previousMembership);
            }

            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = request.UserId,
                JoinedDate = DateTime.UtcNow,
                RemovedDate = null,
                User = user,
                Project = project
            };

            await _projectMemberRepository.AddAsync(projectMember);

            await _projectMemberRepository.SaveChangesAsync();

            return MapToResponseDto(projectMember);
        }

        public async Task RemoveProjectMemberAsync(
            int projectId,
            int projectMemberId,
            int currentUserId)
        {
            var project =
                await _projectRepository.GetByIdAsync(projectId);

            ValidateProjectAndManager(project, currentUserId);

            var projectMember =
                await _projectMemberRepository
                    .GetMembershipByIdAsync(projectMemberId);

            if (projectMember == null ||
                projectMember.ProjectId != projectId)
            {
                throw new KeyNotFoundException(
                    "Project member not found.");
            }

            if (projectMember.RemovedDate != null)
            {
                throw new InvalidOperationException(
                    "This project member has already been removed.");
            }

            projectMember.RemovedDate = DateTime.UtcNow;

            await _projectMemberRepository.SaveChangesAsync();
        }

        private static void ValidateProjectAndManager(
            Project? project,
            int currentUserId)
        {
            if (project == null)
            {
                throw new KeyNotFoundException(
                    "Project not found.");
            }

            if (project.CreatedBy != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the Project Manager who created this project can manage its members.");
            }
        }

        private static ProjectMemberResponseDto MapToResponseDto(
            ProjectMember projectMember)
        {
            return new ProjectMemberResponseDto
            {
                ProjectMemberId =
                    projectMember.ProjectMemberId,

                ProjectId =
                    projectMember.ProjectId,

                UserId =
                    projectMember.UserId,

                FullName =
                    $"{projectMember.User.FirstName} " +
                    $"{projectMember.User.LastName}",

                Email =
                    projectMember.User.Email,

                Role =
                    projectMember.User.Role.ToString(),

                JoinedDate =
                    projectMember.JoinedDate
            };
        }
    }
}