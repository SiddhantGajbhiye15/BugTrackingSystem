using BugTrackingSystem.DTOs.Authentication;
using BugTrackingSystem.DTOs.Users;

public interface IUserService
{
    Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request);

    Task<UserResponseDto?> GetByIdAsync(int userId);

    Task<List<UserResponseDto>> GetAllAsync();

    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
}