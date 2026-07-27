using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        // Admin Dashboard
        Task<int> GetTotalUsersAsync();

        Task<int> GetTotalProjectsAsync();

        Task<List<User>> GetRecentUsersAsync(int count);

        Task<List<Project>> GetProjectsOverviewAsync(int count);

        // Project Manager Dashboard
        Task<List<Project>> GetProjectManagerProjectsAsync(
            int projectManagerId);

        Task<List<Bug>> GetProjectManagerBugsAsync(
            int projectManagerId);
    }
}