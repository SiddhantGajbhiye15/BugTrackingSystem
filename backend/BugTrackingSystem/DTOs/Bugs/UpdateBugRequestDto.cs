using BugTrackingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Bugs
{
    public class UpdateBugRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [EnumDataType(typeof(BugType))]
        public BugType Type { get; set; }

        [EnumDataType(typeof(BugPriority))]
        public BugPriority Priority { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ExpectedOutput { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string ActualOutput { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string StepsToReproduce { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? EvidenceLink { get; set; }
    }
}