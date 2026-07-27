namespace BugTrackingSystem.DTOs.Dashboard
{
    public class TesterDashboardResponseDto
    {
        public TesterCurrentProjectDto? CurrentProject { get; set; }

        public int TotalReportedBugs { get; set; }

        public int OpenBugs { get; set; }

        public int AwaitingVerification { get; set; }

        public int ReopenedBugs { get; set; }

        public BugStatusOverviewDto BugsByStatus { get; set; }
            = new();

        public List<TesterDashboardBugDto> AwaitingVerificationBugs
        { get; set; } = new();

        public List<TesterDashboardBugDto> RecentReportedBugs
        { get; set; } = new();
    }

    public class TesterCurrentProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string ProjectManagerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

    public class TesterDashboardBugDto
    {
        public int BugId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int? AssignedDeveloperId { get; set; }

        public string AssignedDeveloperName { get; set; }
            = "Unassigned";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}