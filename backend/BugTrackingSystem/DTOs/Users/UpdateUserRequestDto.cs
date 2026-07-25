using BugTrackingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Users
{
    public class UpdateUserRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }
}