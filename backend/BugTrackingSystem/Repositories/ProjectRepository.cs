using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories
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
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p =>
                    p.ProjectId == projectId
                );
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
            var hasMembers = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId);

            if (hasMembers)
            {
                return true;
            }

            return await _context.Bugs
                .AnyAsync(b => b.ProjectId == projectId);
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