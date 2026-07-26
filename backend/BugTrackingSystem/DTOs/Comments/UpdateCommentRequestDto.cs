using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.DTOs.Comments
{
    public class UpdateCommentRequestDto
    {
        [Required]
        [MaxLength(1000)]
        public string CommentText { get; set; } = string.Empty;
    }
}