using BugTrackingSystem.DTOs.Authentication;
using BugTrackingSystem.Exceptions;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;

namespace BugTrackingSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            var normalizedEmail = request.Email
                .Trim()
                .ToLowerInvariant();

            var user =
                await _userRepository.GetByEmailAsync(
                    normalizedEmail);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            bool isPasswordValid =
                PasswordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "Your account is inactive. Contact the administrator.");
            }

            string token =
                _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,

                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                }
            };
        }
    }
}