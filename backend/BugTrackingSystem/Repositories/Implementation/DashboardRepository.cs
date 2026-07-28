using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
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
                .Include(u => u.ManagedProjects)
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
                .Include(p => p.ProjectManager)
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
                .Where(p =>
                    p.ProjectManagerId == projectManagerId)
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
                    b.Project.ProjectManagerId == projectManagerId)
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedDeveloper)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProjectMember?>
            GetTesterActiveProjectMembershipAsync(int testerId)
        {
            return await _context.ProjectMembers
                .AsNoTracking()
                .Where(pm =>
                    pm.UserId == testerId &&
                    pm.RemovedDate == null &&
                    pm.Project.Status == ProjectStatus.Active)
                .Include(pm => pm.Project)
                    .ThenInclude(p => p.ProjectManager)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Bug>>
            GetTesterReportedBugsAsync(int testerId)
        {
            return await _context.Bugs
                .AsNoTracking()
                .Where(b => b.ReportedByUserId == testerId)
                .Include(b => b.Project)
                .Include(b => b.AssignedDeveloper)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProjectMember?>
            GetDeveloperActiveProjectMembershipAsync(int developerId)
        {
            return await _context.ProjectMembers
                .AsNoTracking()
                .Where(pm =>
                    pm.UserId == developerId &&
                    pm.RemovedDate == null &&
                    pm.Project.Status == ProjectStatus.Active)
                .Include(pm => pm.Project)
                    .ThenInclude(p => p.ProjectManager)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Bug>>
            GetDeveloperBugsAsync(int developerId)
        {
            return await _context.Bugs
                .AsNoTracking()
                .Where(b =>
                    b.AssignedDeveloperId == developerId)
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
}