using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Projects
{
    public class ChangeProjectManagerRequestDto
    {
        [Range(1, int.MaxValue)]
        public int ProjectManagerId { get; set; }
    }
}