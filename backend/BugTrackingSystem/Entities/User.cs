using BugTrackingSystem.Enums;
namespace BugTrackingSystem.Entities
{
    
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

        public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();

        public ICollection<Bug> ReportedBugs { get; set; } = new List<Bug>();

        public ICollection<Bug> AssignedBugs { get; set; } = new List<Bug>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
