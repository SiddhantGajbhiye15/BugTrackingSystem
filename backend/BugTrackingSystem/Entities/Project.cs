using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;

public class Project
{
    public int ProjectId { get; set; }

    public string ProjectCode { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; }

    // Original creator. This value never changes.
    public int CreatedBy { get; set; }

    // Current Project Manager.
    // Nullable so Admin can assign a manager later.
    public int? ProjectManagerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;

    public User? ProjectManager { get; set; }

    public ICollection<ProjectMember> ProjectMembers { get; set; }
        = new List<ProjectMember>();

    public ICollection<Bug> Bugs { get; set; }
        = new List<Bug>();
}