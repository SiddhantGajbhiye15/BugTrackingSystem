namespace BugTrackingSystem.DTOs.Comments
{
    public class CommentResponseDto
    {
        public int CommentId { get; set; }

        public int BugId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string CommentText { get; set; } = string.Empty;

        public bool IsEdited { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}