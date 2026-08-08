using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Enums;
namespace BugTrackingSystem.Repositories.Implementation
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectManager)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p =>
                    p.ProjectId == projectId);
        }
        


        public async Task<bool> ProjectCodeExistsAsync(
            string projectCode)
        {
            return await _context.Projects.AnyAsync(p =>
                p.ProjectCode == projectCode
            );
        }

        public async Task<bool> HasMembersOrBugsAsync(int projectId)
        {
            var hasAnyMembers = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId);

            if (hasAnyMembers)
            {
                return true;
            }

            var hasAnyBugs = await _context.Bugs
                .AnyAsync(b => b.ProjectId == projectId);

            return hasAnyBugs;
        }
        public async Task<User?> GetUserByIdAsync(int userId)feat: add project lifecycle workflow and preserve membership history
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);

        }

        public void Delete(Project project)
        {
            _context.Projects.Remove(project);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

       
    }
}