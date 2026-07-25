using BugTrackingSystem.DTOs.Authentication;

namespace BugTrackingSystem.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}