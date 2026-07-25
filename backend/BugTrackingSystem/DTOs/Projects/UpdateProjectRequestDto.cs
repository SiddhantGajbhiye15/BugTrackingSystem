using BugTrackingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Projects
{
    public class UpdateProjectRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectStatus Status { get; set; }
    }
}