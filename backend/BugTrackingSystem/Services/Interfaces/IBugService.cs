using BugTrackingSystem.DTOs.Bugs;
using BugTrackingSystem.Enums;

namespace BugTrackingSystem.Services.Interfaces
{
    public interface IBugService
    {
        Task<List<BugResponseDto>> GetProjectBugsAsync(
            int projectId,
            BugStatus? status,
            BugPriority? priority,
            int? assignedDeveloperId,
            int currentUserId,
            UserRole currentUserRole);

        Task<List<BugResponseDto>> GetMyAssignedBugsAsync(
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> GetBugByIdAsync(
            int bugId,
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> CreateBugAsync(
            int projectId,
            CreateBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> UpdateBugAsync(
            int bugId,
            UpdateBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> AssignBugAsync(
            int bugId,
            AssignBugRequestDto request,
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> ChangeBugStatusAsync(
            int bugId,
            ChangeBugStatusRequestDto request,
            int currentUserId,
            UserRole currentUserRole);

        Task<BugResponseDto> ChangeBugPriorityAsync(
            int bugId,
            ChangeBugPriorityRequestDto request,
            int currentUserId,
            UserRole currentUserRole);

        Task DeleteBugAsync(
            int bugId,
            int currentUserId,
            UserRole currentUserRole);
    }
}