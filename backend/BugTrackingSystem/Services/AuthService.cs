using BugTrackingSystem.DTOs.Authentication;
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
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                throw new Exception("Invalid email or password.");

            bool isPasswordValid = PasswordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            string token = _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }
    }
}