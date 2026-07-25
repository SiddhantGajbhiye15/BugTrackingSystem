namespace BugTrackingSystem.DTOs.Authentication
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public UserResponseDto User { get; set; } = null!;
    }
}