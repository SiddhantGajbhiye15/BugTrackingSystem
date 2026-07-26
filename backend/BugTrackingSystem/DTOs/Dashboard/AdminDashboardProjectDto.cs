namespace BugTrackingSystem.DTOs.Dashboard
{
    public class AdminDashboardProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string ProjectManagerName { get; set; } = string.Empty;

        public int ActiveMemberCount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}