using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories.Implementations
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectMember>>
            GetActiveMembersByProjectIdAsync(int projectId)
        {
            return await _context.ProjectMembers
                .AsNoTracking()
                .Include(pm => pm.User)
                .Where(pm =>
                    pm.ProjectId == projectId &&
                    pm.RemovedDate == null)
                .OrderBy(pm => pm.User.FirstName)
                .ThenBy(pm => pm.User.LastName)
                .ToListAsync();
        }

        public async Task<ProjectMember?>
            GetActiveMembershipByUserIdAsync(int userId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.Project)
                .FirstOrDefaultAsync(pm =>
                    pm.UserId == userId &&
                    pm.RemovedDate == null);
        }

        public async Task<ProjectMember?> GetMembershipAsync(
            int projectId,
            int userId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.User)
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == userId);
        }

        public async Task<ProjectMember?> GetMembershipByIdAsync(
            int projectMemberId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.User)
                .Include(pm => pm.Project)
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectMemberId == projectMemberId);
        }

        public async Task<List<User>> GetAvailableUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.IsActive &&
                    (u.Role == UserRole.Developer ||
                     u.Role == UserRole.Tester) &&
                    !u.ProjectMemberships.Any(pm =>
                        pm.RemovedDate == null))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task AddAsync(ProjectMember projectMember)
        {
            await _context.ProjectMembers.AddAsync(projectMember);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}