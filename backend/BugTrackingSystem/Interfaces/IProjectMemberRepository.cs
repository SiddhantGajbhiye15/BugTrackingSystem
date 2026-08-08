using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Repositories.Interfaces
{
    public interface IProjectMemberRepository
    {
        Task<List<ProjectMember>> GetActiveMembersByProjectIdAsync(
            int projectId);

        Task<ProjectMember?> GetActiveMembershipByUserIdAsync(
            int userId);

        Task<ProjectMember?> GetMembershipAsync(
            int projectId,
            int userId);

        Task<ProjectMember?> GetMembershipByIdAsync(
            int projectMemberId);

        Task<List<User>> GetAvailableUsersAsync();

        Task<List<ProjectMember>> GetActiveMembersForUpdateAsync(
            int projectId);

        Task AddAsync(ProjectMember projectMember);

        Task SaveChangesAsync();
    }
}