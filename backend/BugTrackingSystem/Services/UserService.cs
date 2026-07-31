using BugTrackingSystem.DTOs.Authentication;
using BugTrackingSystem.DTOs.Users;
using BugTrackingSystem.Entities;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;

namespace BugTrackingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> CreateUserAsync(
            CreateUserRequestDto request)
        {
            var normalizedEmail = request.Email
                .Trim()
                .ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(normalizedEmail))
            {
                throw new InvalidOperationException(
                    "Email already exists."
                );
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = normalizedEmail,
                PasswordHash = PasswordHasher.HashPassword(
                    request.Password
                ),
                Role = request.Role,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(
            int userId,
            UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var normalizedEmail = request.Email
                .Trim()
                .ToLowerInvariant();

            var emailExists =
                await _userRepository.EmailExistsAsync(
                    normalizedEmail,
                    userId
                );

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "Email already exists."
                );
            }

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = normalizedEmail;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateUserAsync(
            int userId,
            int currentAdminUserId)
        {
            if (userId == currentAdminUserId)
            {
                throw new InvalidOperationException(
                    "You cannot deactivate your own account."
                );
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetPasswordAsync(
            int userId,
            ResetPasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.PasswordHash = PasswordHasher.HashPassword(
                request.NewPassword
            );

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task ChangePasswordAsync(
            int userId,
            ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "User not found."
                );
            }

            var currentPasswordIsCorrect =
                PasswordHasher.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash
                );

            if (!currentPasswordIsCorrect)
            {
                throw new InvalidOperationException(
                    "Current password is incorrect."
                );
            }

            user.PasswordHash = PasswordHasher.HashPassword(
                request.NewPassword
            );

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}