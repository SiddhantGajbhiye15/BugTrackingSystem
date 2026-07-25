using BugTrackingSystem.Enums;

namespace BugTrackingSystem.DTOs.Projects
{
    public class ProjectResponseDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ProjectStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}