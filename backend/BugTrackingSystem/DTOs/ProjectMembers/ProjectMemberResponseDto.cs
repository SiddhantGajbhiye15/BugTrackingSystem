namespace BugTrackingSystem.DTOs.ProjectMembers
{
    public class ProjectMemberResponseDto
    {
        public int ProjectMemberId { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; }
    }
}