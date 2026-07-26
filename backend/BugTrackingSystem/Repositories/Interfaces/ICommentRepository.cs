using BugTrackingSystem.Entities;

namespace BugTrackingSystem.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetBugCommentsAsync(int bugId);

        Task<Comment?> GetByIdAsync(int commentId);

        Task<Bug?> GetBugWithAccessDataAsync(int bugId);

        Task AddAsync(Comment comment);

        void Delete(Comment comment);

        Task SaveChangesAsync();
    }
}