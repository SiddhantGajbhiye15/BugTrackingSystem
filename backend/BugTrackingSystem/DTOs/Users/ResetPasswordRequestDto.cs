using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Users
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [MinLength(
            8,
            ErrorMessage = "Password must be at least 8 characters long."
        )]
        [MaxLength(
            100,
            ErrorMessage = "Password cannot exceed 100 characters."
        )]
        public string NewPassword { get; set; } = string.Empty;
    }
}