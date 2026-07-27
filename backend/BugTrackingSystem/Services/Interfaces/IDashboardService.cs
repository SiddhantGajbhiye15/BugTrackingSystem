using BugTrackingSystem.DTOs.Dashboard;

namespace BugTrackingSystem.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardResponseDto>
            GetAdminDashboardAsync();

        Task<ProjectManagerDashboardResponseDto>
            GetProjectManagerDashboardAsync(
                int currentUserId);

        Task<TesterDashboardResponseDto>
            GetTesterDashboardAsync(
                int currentUserId);
        Task<DeveloperDashboardResponseDto>
            GetDeveloperDashboardAsync(int currentUserId);
    }
}