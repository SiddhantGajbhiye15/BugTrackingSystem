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
        public async Task<ProjectManagerDashboardResponseDto>
    GetProjectManagerDashboardAsync(int currentUserId)
        {
            var projects = await _dashboardRepository
                .GetProjectManagerProjectsAsync(currentUserId);

            var bugs = await _dashboardRepository
                .GetProjectManagerBugsAsync(currentUserId);

            var unassignedAndUrgentBugs = bugs
                .Where(b =>
                    b.Status != BugStatus.Closed &&
                    (
                        b.AssignedDeveloperId == null ||
                        b.Priority == BugPriority.Critical
                    ))
                .OrderByDescending(b => b.Priority)
                .ThenBy(b => b.CreatedAt)
                .Take(5)
                .Select(MapProjectManagerBug)
                .ToList();

            var projectsOverview = projects
                .Take(5)
                .Select(project =>
                {
                    var projectBugs = bugs
                        .Where(b =>
                            b.ProjectId == project.ProjectId)
                        .ToList();

                    return new ProjectManagerProjectDto
                    {
                        ProjectId = project.ProjectId,

                        ProjectCode = project.ProjectCode,

                        ProjectName = project.ProjectName,

                        Status = project.Status.ToString(),

                        ActiveMemberCount =
                            project.ProjectMembers.Count(pm =>
                                pm.RemovedDate == null),

                        OpenBugCount = projectBugs.Count(b =>
                            b.Status == BugStatus.Open),

                        CriticalBugCount = projectBugs.Count(b =>
                            b.Priority == BugPriority.Critical &&
                            b.Status != BugStatus.Closed)
                    };
                })
                .ToList();

            var developerWorkload = projects
                .SelectMany(project =>
                    project.ProjectMembers
                        .Where(pm =>
                            pm.RemovedDate == null &&
                            pm.User.Role == UserRole.Developer)
                        .Select(pm =>
                        {
                            var developerBugs = bugs
                                .Where(b =>
                                    b.ProjectId == project.ProjectId &&
                                    b.AssignedDeveloperId == pm.UserId)
                                .ToList();

                            return new DeveloperWorkloadDto
                            {
                                DeveloperId = pm.UserId,

                                DeveloperName =
                                    $"{pm.User.FirstName} " +
                                    $"{pm.User.LastName}".Trim(),

                                ProjectId = project.ProjectId,

                                ProjectName = project.ProjectName,

                                AssignedCount = developerBugs.Count(b =>
                                    b.Status == BugStatus.Assigned),

                                InProgressCount = developerBugs.Count(b =>
                                    b.Status == BugStatus.InProgress),

                                ResolvedCount = developerBugs.Count(b =>
                                    b.Status == BugStatus.Resolved)
                            };
                        }))
                .OrderByDescending(d =>
                    d.AssignedCount + d.InProgressCount)
                .Take(5)
                .ToList();

            return new ProjectManagerDashboardResponseDto
            {
                TotalProjects = projects.Count,

                OpenBugs = bugs.Count(b =>
                    b.Status == BugStatus.Open),

                UnassignedBugs = bugs.Count(b =>
                    b.AssignedDeveloperId == null &&
                    b.Status != BugStatus.Closed),

                CriticalBugs = bugs.Count(b =>
                    b.Priority == BugPriority.Critical &&
                    b.Status != BugStatus.Closed),

                BugsByStatus = new BugStatusOverviewDto
                {
                    Open = bugs.Count(b =>
                        b.Status == BugStatus.Open),

                    Assigned = bugs.Count(b =>
                        b.Status == BugStatus.Assigned),

                    InProgress = bugs.Count(b =>
                        b.Status == BugStatus.InProgress),

                    Resolved = bugs.Count(b =>
                        b.Status == BugStatus.Resolved),

                    Closed = bugs.Count(b =>
                        b.Status == BugStatus.Closed),

                    Reopened = bugs.Count(b =>
                        b.Status == BugStatus.Reopened)
                },

                UnassignedAndUrgentBugs =
                    unassignedAndUrgentBugs,

                ProjectsOverview = projectsOverview,

                DeveloperWorkload = developerWorkload,

                RecentBugs = bugs
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .Select(MapProjectManagerBug)
                    .ToList()
            };
        }
        private static ProjectManagerBugDto MapProjectManagerBug(Bug bug)
        {
            string reporterName =
                $"{bug.ReportedByUser.FirstName} " +
                $"{bug.ReportedByUser.LastName}";

            string assignedDeveloperName = "Unassigned";

            if (bug.AssignedDeveloper != null)
            {
                assignedDeveloperName =
                    $"{bug.AssignedDeveloper.FirstName} " +
                    $"{bug.AssignedDeveloper.LastName}";
            }

            return new ProjectManagerBugDto
            {
                BugId = bug.BugId,

                Title = bug.Title,

                ProjectId = bug.ProjectId,

                ProjectName = bug.Project.ProjectName,

                Priority = bug.Priority.ToString(),

                Status = bug.Status.ToString(),

                ReporterName = reporterName.Trim(),

                AssignedDeveloperId =
                    bug.AssignedDeveloperId,

                AssignedDeveloperName =
                    assignedDeveloperName.Trim(),

                CreatedAt = bug.CreatedAt,

                UpdatedAt = bug.UpdatedAt
            };
        }
    }
}