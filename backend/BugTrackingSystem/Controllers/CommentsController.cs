using System.Security.Claims;
using BugTrackingSystem.DTOs.Comments;
using BugTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /api/bugs/{bugId}/comments
        [HttpGet("bugs/{bugId:int}/comments")]
        public async Task<ActionResult<List<CommentResponseDto>>>
            GetBugComments(int bugId)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                string currentUserRole = GetCurrentUserRole();

                var comments =
                    await _commentService.GetBugCommentsAsync(
                        bugId,
                        currentUserId,
                        currentUserRole);

                return Ok(comments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // POST: /api/bugs/{bugId}/comments
        [HttpPost("bugs/{bugId:int}/comments")]
        public async Task<ActionResult<CommentResponseDto>>
            CreateComment(
                int bugId,
                CreateCommentRequestDto request)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                string currentUserRole = GetCurrentUserRole();

                var comment =
                    await _commentService.CreateCommentAsync(
                        bugId,
                        request,
                        currentUserId,
                        currentUserRole);

                return CreatedAtAction(
                    nameof(GetBugComments),
                    new { bugId },
                    comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // PUT: /api/comments/{commentId}
        [HttpPut("comments/{commentId:int}")]
        public async Task<ActionResult<CommentResponseDto>>
            UpdateComment(
                int commentId,
                UpdateCommentRequestDto request)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                string currentUserRole = GetCurrentUserRole();

                var comment =
                    await _commentService.UpdateCommentAsync(
                        commentId,
                        request,
                        currentUserId,
                        currentUserRole);

                return Ok(comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        // DELETE: /api/comments/{commentId}
        [HttpDelete("comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                string currentUserRole = GetCurrentUserRole();

                await _commentService.DeleteCommentAsync(
                    commentId,
                    currentUserId,
                    currentUserRole);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            string? userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return currentUserId;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role)
                ?? throw new UnauthorizedAccessException(
                    "User role was not found in the token.");
        }
    }
}