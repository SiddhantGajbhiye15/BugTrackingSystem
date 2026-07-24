namespace BugTrackingSystem.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }

        public int BugId { get; set; }

        public int UserId { get; set; }

        public string CommentText { get; set; } = string.Empty;

        public bool IsEdited { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties

        public Bug Bug { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
