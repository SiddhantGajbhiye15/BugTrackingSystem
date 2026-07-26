namespace BugTrackingSystem.DTOs.Bugs
{
    public class BugResponseDto
    {
        public int BugId { get; set; }

        public int ProjectId { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string ExpectedOutput { get; set; } = string.Empty;

        public string ActualOutput { get; set; } = string.Empty;

        public string StepsToReproduce { get; set; } = string.Empty;

        public string? EvidenceLink { get; set; }

        public int ReportedByUserId { get; set; }

        public string ReporterName { get; set; } = string.Empty;

        public int? AssignedDeveloperId { get; set; }

        public string? AssignedDeveloperName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}