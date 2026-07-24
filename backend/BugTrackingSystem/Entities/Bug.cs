
using BugTrackingSystem.Enums;
namespace BugTrackingSystem.Entities

{
    public class Bug
    {
        public int BugId { get; set; }

        public int ProjectId { get; set; }

        public int ReportedByUserId { get; set; }

        public int? AssignedDeveloperId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public BugPriority Priority { get; set; }

        public BugStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        // Navigation Properties

        public Project Project { get; set; } = null!;

        public User ReportedByUser { get; set; } = null!;

        public User? AssignedDeveloper { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
