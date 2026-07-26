using BugTrackingSystem.DTOs.ProjectMembers;

namespace BugTrackingSystem.Services.Interfaces
{
    public interface IProjectMemberService
    {
        Task<List<ProjectMemberResponseDto>> GetProjectMembersAsync(
            int projectId,
            int currentUserId);

        Task<List<AvailableUserResponseDto>> GetAvailableUsersAsync(
            int projectId,
            int currentUserId);

        Task<ProjectMemberResponseDto> AddProjectMemberAsync(
            int projectId,
            AddProjectMemberRequestDto request,
            int currentUserId);

        Task RemoveProjectMemberAsync(
            int projectId,
            int projectMemberId,
            int currentUserId);
    }
}