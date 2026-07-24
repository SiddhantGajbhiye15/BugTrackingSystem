using BugTrackingSystem.Entities;
using BugTrackingSystem.Enums;

public class Project
{
    public int ProjectId { get; set; }

    public string ProjectCode { get; set; } = string.Empty;

    public string ProjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties

    public User CreatedByUser { get; set; } = null!;

    public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public ICollection<Bug> Bugs { get; set; } = new List<Bug>();
}