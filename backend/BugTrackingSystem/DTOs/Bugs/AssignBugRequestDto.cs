using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Bugs
{
    public class AssignBugRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid developer ID is required.")]
        public int DeveloperId { get; set; }
    }
}