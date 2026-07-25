using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();

        Task<Project?> GetByIdAsync(int projectId);

        Task<bool> ProjectCodeExistsAsync(string projectCode);

        Task<bool> HasMembersOrBugsAsync(int projectId);

        Task AddAsync(Project project);

        void Delete(Project project);

        Task SaveChangesAsync();
    }
}