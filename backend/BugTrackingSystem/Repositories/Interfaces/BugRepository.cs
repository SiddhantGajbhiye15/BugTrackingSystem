using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;
using BugTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories.Implementations
{
    public class BugRepository : IBugRepository
    {
        private readonly ApplicationDbContext _context;

        public BugRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bug>> GetProjectBugsAsync(
            int projectId,
            BugStatus? status = null,
            BugPriority? priority = null,
            int? assignedDeveloperId = null)
        {
            var query = _context.Bugs
                .AsNoTracking()
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedDeveloper)
                .Where(b => b.ProjectId == projectId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(b => b.Priority == priority.Value);
            }

            if (assignedDeveloperId.HasValue)
            {
                query = query.Where(
                    b => b.AssignedDeveloperId == assignedDeveloperId.Value);
            }

            return await query
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Bug>> GetAssignedBugsAsync(
            int developerId)
        {
            return await _context.Bugs
                .AsNoTracking()
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedDeveloper)
                .Where(b =>
                    b.AssignedDeveloperId == developerId &&
                    b.Status != BugStatus.Closed)
                .OrderByDescending(b => b.Priority)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bug?> GetByIdAsync(int bugId)
        {
            return await _context.Bugs
                .Include(b => b.Project)
                .Include(b => b.ReportedByUser)
                .Include(b => b.AssignedDeveloper)
                .FirstOrDefaultAsync(b => b.BugId == bugId);
        }

        public async Task<bool> HasCommentsAsync(int bugId)
        {
            return await _context.Comments
                .AnyAsync(c => c.BugId == bugId);
        }

        public async Task AddAsync(Bug bug)
        {
            await _context.Bugs.AddAsync(bug);
        }

        public void Delete(Bug bug)
        {
            _context.Bugs.Remove(bug);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}