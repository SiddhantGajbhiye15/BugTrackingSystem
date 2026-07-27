namespace BugTrackingSystem.DTOs.Dashboard
{
    public class DeveloperDashboardResponseDto
    {
        public DeveloperCurrentProjectDto? CurrentProject { get; set; }

        public int AssignedBugs { get; set; }

        public int InProgressBugs { get; set; }

        public int ResolvedBugs { get; set; }

        public int CriticalActiveBugs { get; set; }

        public DeveloperBugStatusOverviewDto BugsByStatus { get; set; }
            = new();

        public DeveloperPriorityOverviewDto ActiveBugsByPriority { get; set; }
            = new();

        public List<DeveloperDashboardBugDto> ActiveBugs { get; set; }
            = new();

        public List<DeveloperDashboardBugDto> RecentlyResolvedBugs
        { get; set; } = new();
    }

    public class DeveloperCurrentProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string ProjectManagerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

    public class DeveloperBugStatusOverviewDto
    {
        public int Assigned { get; set; }

        public int InProgress { get; set; }

        public int Resolved { get; set; }

        public int Closed { get; set; }
    }

    public class DeveloperPriorityOverviewDto
    {
        public int Low { get; set; }

        public int Medium { get; set; }

        public int High { get; set; }

        public int Critical { get; set; }
    }

    public class DeveloperDashboardBugDto
    {
        public int BugId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int ReporterId { get; set; }

        public string ReporterName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}