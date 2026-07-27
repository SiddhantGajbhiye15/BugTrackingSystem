using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- ADMIN DASHBOARD ----------------

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalProjectsAsync()
        {
            return await _context.Projects.CountAsync();
        }

        public async Task<List<User>> GetRecentUsersAsync(int count)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.ProjectMemberships
                    .Where(pm => pm.RemovedDate == null))
                    .ThenInclude(pm => pm.Project)
                .Include(u => u.CreatedProjects)
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsOverviewAsync(
            int count)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectMembers
                    .Where(pm => pm.RemovedDate == null))
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .AsSplitQuery()
                .ToListAsync();
        }

        // ------------- PROJECT MANAGER DASHBOARD -------------

        public async Task<List<Project>>
            GetProjectManagerProjectsAsync(int projectManagerId)
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.CreatedBy == projectManagerId)
                .Include(p => p.ProjectMembers
                    .Where(pm => pm.RemovedDate == null))
                    .ThenInclude(pm => pm.User)
                .OrderByDescending(p => p.CreatedAt)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Bug>>
            GetProjectManagerBugsAsync(int projectManagerId)
        {
            return await _context.Bugs
                .AsNoTracking()
                .Where(b =>
                    b.Project.CreatedBy == projectManagerId)
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedDeveloper)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
}