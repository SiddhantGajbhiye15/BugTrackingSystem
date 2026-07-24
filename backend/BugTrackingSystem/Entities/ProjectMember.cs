namespace BugTrackingSystem.Entities
{
    public class ProjectMember
    {
        public int ProjectMemberId { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        public User User { get; set; } = null!;

        public Project Project { get; set; } = null!;

    }
}
