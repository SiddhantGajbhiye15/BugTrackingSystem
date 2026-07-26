using BugTrackingSystem.DTOs.Dashboard;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Repositories.Interfaces;
using BugTrackingSystem.Services.Interfaces;

namespace BugTrackingSystem.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<AdminDashboardResponseDto>
            GetAdminDashboardAsync()
        {
            int totalUsers =
                await _dashboardRepository.GetTotalUsersAsync();

            int totalProjects =
                await _dashboardRepository.GetTotalProjectsAsync();

            var recentUsers =
                await _dashboardRepository.GetRecentUsersAsync(5);

            var projects =
                await _dashboardRepository.GetProjectsOverviewAsync(5);

            return new AdminDashboardResponseDto
            {
                TotalUsers = totalUsers,

                TotalProjects = totalProjects,

                RecentUsers = recentUsers
                    .Select(MapUser)
                    .ToList(),

                ProjectsOverview = projects
                    .Select(MapProject)
                    .ToList()
            };
        }

        private static AdminDashboardUserDto MapUser(User user)
        {
            return new AdminDashboardUserDto
            {
                UserId = user.UserId,

                FullName =
                    $"{user.FirstName} {user.LastName}".Trim(),

                Email = user.Email,

                Role = user.Role.ToString(),

                CurrentProject = GetCurrentProject(user),

                IsActive = user.IsActive,

                CreatedAt = user.CreatedAt
            };
        }

        private static string GetCurrentProject(User user)
        {
            if (user.Role == UserRole.Admin)
            {
                return "—";
            }

            if (user.Role == UserRole.ProjectManager)
            {
                int projectCount = user.CreatedProjects.Count;

                if (projectCount == 0)
                {
                    return "No Projects";
                }

                if (projectCount == 1)
                {
                    return user.CreatedProjects
                        .First()
                        .ProjectName;
                }

                return $"{projectCount} Projects";
            }

            var activeMembership = user.ProjectMemberships
                .FirstOrDefault(pm => pm.RemovedDate == null);

            return activeMembership?.Project?.ProjectName
                ?? "Unassigned";
        }

        private static AdminDashboardProjectDto MapProject(
            Project project)
        {
            return new AdminDashboardProjectDto
            {
                ProjectId = project.ProjectId,

                ProjectCode = project.ProjectCode,

                ProjectName = project.ProjectName,

               
                ProjectManagerName =
                    $"{project.CreatedByUser.FirstName} " +
                    $"{project.CreatedByUser.LastName}",

                ActiveMemberCount = project.ProjectMembers.Count(
                    pm => pm.RemovedDate == null),

                Status = project.Status.ToString(),

                CreatedAt = project.CreatedAt
            };
        }
    }
}