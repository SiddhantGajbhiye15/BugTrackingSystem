using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Projects
{
    public class CreateProjectRequestDto
    {
        [Required]
        [MaxLength(20)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}