using BugTrackingSystem.DTOs.Authentication;
using BugTrackingSystem.DTOs.Users;

namespace BugTrackingSystem.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(
            CreateUserRequestDto request
        );

        Task<List<UserResponseDto>> GetAllAsync();

        Task<UserResponseDto?> GetByIdAsync(int userId);

        Task<UserResponseDto?> UpdateUserAsync(
            int userId,
            UpdateUserRequestDto request
        );

        Task<bool> ActivateUserAsync(int userId);

        Task<bool> DeactivateUserAsync(
            int userId,
            int currentAdminUserId
        );

        Task ChangePasswordAsync(
            int userId,
            ChangePasswordRequestDto request
        );
    }
}