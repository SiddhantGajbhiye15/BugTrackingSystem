using BugTrackingSystem.Enums;

namespace BugTrackingSystem.DTOs.Authentication
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }
    }
}