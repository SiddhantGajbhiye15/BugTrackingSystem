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

        Task<bool> DeleteAsync(
            int projectId,
            int currentUserId
        );
    }
}