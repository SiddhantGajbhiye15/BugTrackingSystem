using BugTrackingSystem.DTOs.Projects;

namespace BugTrackingSystem.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponseDto> CreateAsync(
            CreateProjectRequestDto request,
            int currentUserId
        );

        Task<List<ProjectResponseDto>> GetAllAsync();

        Task<ProjectResponseDto?> GetByIdAsync(int projectId);

        Task<ProjectResponseDto?> UpdateAsync(
            int projectId,
            UpdateProjectRequestDto request,
            int currentUserId
        );
        Task<ProjectResponseDto?> ChangeManagerAsync(
            int projectId,
            ChangeProjectManagerRequestDto request);
        Task<bool> DeleteAsync(
            int projectId,
            int currentUserId
        );
        Task<ProjectResponseDto> CompleteAsync(
    int projectId,
    int currentUserId);

        Task<ProjectResponseDto> ArchiveAsync(
            int projectId,
            int currentUserId);

        Task<ProjectResponseDto> RestoreAsync(
            int projectId,
            int currentUserId);
    }
}