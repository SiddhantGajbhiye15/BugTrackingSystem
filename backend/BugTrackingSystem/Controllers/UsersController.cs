using BugTrackingSystem.DTOs.Users;
using BugTrackingSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Only Admin can create users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(
            CreateUserRequestDto request)
        {
            try
            {
                var response =
                    await _userService.CreateUserAsync(request);

                return Ok(response);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        // Only Admin can view all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        // Only Admin can view a user by ID
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = "User not found."
                });
            }

            return Ok(user);
        }

        // Only Admin can update a user
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(
            int id,
            UpdateUserRequestDto request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(
                    id,
                    request
                );

                if (user == null)
                {
                    return NotFound(new
                    {
                        Message = "User not found."
                    });
                }

                return Ok(user);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        // Only Admin can activate a user
        [HttpPatch("{id:int}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var activated =
                await _userService.ActivateUserAsync(id);

            if (!activated)
            {
                return NotFound(new
                {
                    Message = "User not found."
                });
            }

            return Ok(new
            {
                Message = "User activated successfully."
            });
        }

        // Only Admin can deactivate a user
        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var currentAdminId = GetCurrentUserId();

            try
            {
                var deactivated =
                    await _userService.DeactivateUserAsync(
                        id,
                        currentAdminId
                    );

                if (!deactivated)
                {
                    return NotFound(new
                    {
                        Message = "User not found."
                    });
                }

                return Ok(new
                {
                    Message = "User deactivated successfully."
                });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        // Any logged-in user can view their own profile
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();

            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    Message = "User not found."
                });
            }

            return Ok(user);
        }

        // Any logged-in user can change their own password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordRequestDto request)
        {
            var userId = GetCurrentUserId();

            try
            {
                await _userService.ChangePasswordAsync(
                    userId,
                    request
                );

                return Ok(new
                {
                    Message = "Password changed successfully."
                });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    Message = exception.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            return int.Parse(userIdValue!);
        }
    }
}