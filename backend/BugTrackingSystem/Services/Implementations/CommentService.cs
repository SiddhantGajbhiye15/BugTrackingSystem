using BugTrackingSystem.DTOs.Comments;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Repositories.Interfaces;
using BugTrackingSystem.Services.Interfaces;

namespace BugTrackingSystem.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(
            ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<List<CommentResponseDto>> GetBugCommentsAsync(
            int bugId,
            int currentUserId,
            string currentUserRole)
        {
            var bug = await _commentRepository
                .GetBugWithAccessDataAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectAccess(
                bug,
                currentUserId,
                currentUserRole);

            var comments = await _commentRepository
                .GetBugCommentsAsync(bugId);

            return comments
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<CommentResponseDto> CreateCommentAsync(
            int bugId,
            CreateCommentRequestDto request,
            int currentUserId,
            string currentUserRole)
        {
            var commentText = request.CommentText.Trim();

            if (string.IsNullOrWhiteSpace(commentText))
            {
                throw new ArgumentException(
                    "Comment cannot be empty.");
            }

            var bug = await _commentRepository
                .GetBugWithAccessDataAsync(bugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectAccess(
                bug,
                currentUserId,
                currentUserRole);

            var comment = new Comment
            {
                BugId = bugId,
                UserId = currentUserId,
                CommentText = commentText,
                IsEdited = false,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            var createdComment = await _commentRepository
                .GetByIdAsync(comment.CommentId);

            if (createdComment == null)
            {
                throw new InvalidOperationException(
                    "Comment was created but could not be retrieved.");
            }

            return MapToResponseDto(createdComment);
        }

        public async Task<CommentResponseDto> UpdateCommentAsync(
            int commentId,
            UpdateCommentRequestDto request,
            int currentUserId,
            string currentUserRole)
        {
            var commentText = request.CommentText.Trim();

            if (string.IsNullOrWhiteSpace(commentText))
            {
                throw new ArgumentException(
                    "Comment cannot be empty.");
            }

            var comment = await _commentRepository
                .GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException(
                    "Comment not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can edit only your own comments.");
            }

            var bug = await _commentRepository
                .GetBugWithAccessDataAsync(comment.BugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectAccess(
                bug,
                currentUserId,
                currentUserRole);

            comment.CommentText = commentText;
            comment.IsEdited = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.SaveChangesAsync();

            return MapToResponseDto(comment);
        }

        public async Task DeleteCommentAsync(
            int commentId,
            int currentUserId,
            string currentUserRole)
        {
            var comment = await _commentRepository
                .GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException(
                    "Comment not found.");
            }

            if (comment.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can delete only your own comments.");
            }

            var bug = await _commentRepository
                .GetBugWithAccessDataAsync(comment.BugId);

            if (bug == null)
            {
                throw new KeyNotFoundException("Bug not found.");
            }

            EnsureProjectAccess(
                bug,
                currentUserId,
                currentUserRole);

            _commentRepository.Delete(comment);
            await _commentRepository.SaveChangesAsync();
        }

        private static void EnsureProjectAccess(
            Bug bug,
            int currentUserId,
            string currentUserRole)
        {
            if (currentUserRole == "Admin")
            {
                return;
            }

            bool isProjectCreator =
                bug.Project.CreatedBy == currentUserId;

            bool isActiveMember =
                bug.Project.ProjectMembers.Any(pm =>
                    pm.UserId == currentUserId &&
                    pm.RemovedDate == null);

            if (!isProjectCreator && !isActiveMember)
            {
                throw new UnauthorizedAccessException(
                    "You do not have access to this bug's comments.");
            }
        }

        private static CommentResponseDto MapToResponseDto(
            Comment comment)
        {
            return new CommentResponseDto
            {
                CommentId = comment.CommentId,
                BugId = comment.BugId,
                UserId = comment.UserId,
                UserName =
                    $"{comment.User.FirstName} {comment.User.LastName}".Trim(),
                CommentText = comment.CommentText,
                IsEdited = comment.IsEdited,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}