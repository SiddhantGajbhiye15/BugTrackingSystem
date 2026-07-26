using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;

namespace BugTrackingSystem.Repositories.Interfaces
{
    public interface IBugRepository
    {
        Task<List<Bug>> GetProjectBugsAsync(
            int projectId,
            BugStatus? status = null,
            BugPriority? priority = null,
            int? assignedDeveloperId = null);

        Task<List<Bug>> GetAssignedBugsAsync(int developerId);

        Task<Bug?> GetByIdAsync(int bugId);

        Task<bool> HasCommentsAsync(int bugId);

        Task AddAsync(Bug bug);

        void Delete(Bug bug);

        Task SaveChangesAsync();
    }
}