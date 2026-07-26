using BugTrackingSystem.Data;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetBugCommentsAsync(int bugId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.BugId == bugId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Bug)
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId);
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Bug?> GetBugWithAccessDataAsync(int bugId)
        {
            return await _context.Bugs
                .AsNoTracking()
                .Include(b => b.Project)
                    .ThenInclude(p => p.ProjectMembers)
                .FirstOrDefaultAsync(b => b.BugId == bugId);
        }
    }
}