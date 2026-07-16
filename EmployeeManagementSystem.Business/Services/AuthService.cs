using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // User not found
            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password.",
                    MustChangePassword = false
                };
            }

            // Verify password
            bool isPasswordValid = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash);

            if (!isPasswordValid)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password.",
                    MustChangePassword = false
                };
            }

            // Check if account is deleted
            if (user.IsDeleted)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "This account has been deleted.",
                    MustChangePassword = false
                };
            }

            // Check if account is inactive
            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "This account is disabled.",
                    MustChangePassword = false
                };
            }

            // Login successful
            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                MustChangePassword = user.MustChangePassword
            };
        }

    }

}
