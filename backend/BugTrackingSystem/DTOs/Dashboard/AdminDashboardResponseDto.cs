namespace BugTrackingSystem.DTOs.Dashboard
{
    public class AdminDashboardResponseDto
    {
        public int TotalUsers { get; set; }

        public int TotalProjects { get; set; }

        public List<AdminDashboardUserDto> RecentUsers { get; set; }
            = new();

        public List<AdminDashboardProjectDto> ProjectsOverview { get; set; }
            = new();
    }
}