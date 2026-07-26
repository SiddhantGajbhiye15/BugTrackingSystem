using BugTrackingSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Bugs
{
    public class ChangeBugStatusRequestDto
    {
        [EnumDataType(
            typeof(BugStatus),
            ErrorMessage = "Invalid bug status.")]
        public BugStatus Status { get; set; }
    }
}