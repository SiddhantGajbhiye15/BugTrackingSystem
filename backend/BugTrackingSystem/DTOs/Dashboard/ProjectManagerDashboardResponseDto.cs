namespace BugTrackingSystem.DTOs.Dashboard
{
    public class ProjectManagerDashboardResponseDto
    {
        public int TotalProjects { get; set; }

        public int OpenBugs { get; set; }

        public int UnassignedBugs { get; set; }

        public int CriticalBugs { get; set; }

        public BugStatusOverviewDto BugsByStatus { get; set; }
            = new();

        public List<ProjectManagerBugDto> UnassignedAndUrgentBugs
        { get; set; } = new();

        public List<ProjectManagerProjectDto> ProjectsOverview
        { get; set; } = new();

        public List<DeveloperWorkloadDto> DeveloperWorkload
        { get; set; } = new();

        public List<ProjectManagerBugDto> RecentBugs
        { get; set; } = new();
    }

    public class BugStatusOverviewDto
    {
        public int Open { get; set; }

        public int Assigned { get; set; }

        public int InProgress { get; set; }

        public int Resolved { get; set; }

        public int Closed { get; set; }

        public int Reopened { get; set; }
    }

    public class ProjectManagerBugDto
    {
        public int BugId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string ReporterName { get; set; } = string.Empty;

        public int? AssignedDeveloperId { get; set; }

        public string AssignedDeveloperName { get; set; }
            = "Unassigned";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class ProjectManagerProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int ActiveMemberCount { get; set; }

        public int OpenBugCount { get; set; }

        public int CriticalBugCount { get; set; }
    }

    public class DeveloperWorkloadDto
    {
        public int DeveloperId { get; set; }

        public string DeveloperName { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public int AssignedCount { get; set; }

        public int InProgressCount { get; set; }

        public int ResolvedCount { get; set; }
    }
}