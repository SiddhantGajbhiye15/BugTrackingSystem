using BugTrackingSystem.DTOs.Comments;

namespace BugTrackingSystem.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentResponseDto>> GetBugCommentsAsync(
            int bugId,
            int currentUserId,
            string currentUserRole);

        Task<CommentResponseDto> CreateCommentAsync(
            int bugId,
            CreateCommentRequestDto request,
            int currentUserId,
            string currentUserRole);

        Task<CommentResponseDto> UpdateCommentAsync(
            int commentId,
            UpdateCommentRequestDto request,
            int currentUserId,
            string currentUserRole);

        Task DeleteCommentAsync(
            int commentId,
            int currentUserId,
            string currentUserRole);
    }
}