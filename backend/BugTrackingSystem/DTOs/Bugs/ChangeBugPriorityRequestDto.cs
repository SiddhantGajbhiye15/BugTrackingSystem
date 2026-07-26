using BugTrackingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Bugs
{
    public class ChangeBugPriorityRequestDto
    {
        [EnumDataType(
            typeof(BugPriority),
            ErrorMessage = "Invalid bug priority.")]
        public BugPriority Priority { get; set; }
    }
}