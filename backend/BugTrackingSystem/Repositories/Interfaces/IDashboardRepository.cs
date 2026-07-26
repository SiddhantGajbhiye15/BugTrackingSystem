using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalUsersAsync();

        Task<int> GetTotalProjectsAsync();

        Task<List<User>> GetRecentUsersAsync(int count);

        Task<List<Project>> GetProjectsOverviewAsync(int count);
    }
}