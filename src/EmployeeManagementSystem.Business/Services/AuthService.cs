
using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Interfaces;
using System.Security.Claims;

namespace EmployeeManagementSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPasswordService _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenGraceCache _graceCache;

        public AuthService(IEmployeeRepository employeeRepository, IPasswordService passwordHasher, IJwtService jwtService, IRefreshTokenGraceCache cache)
        {
            _employeeRepository = employeeRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _graceCache = cache;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var employee = await _employeeRepository.GetByEmailAsync(request.Email);

            // User not found
            if (employee == null)
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
                employee.PasswordHash);

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
            if (employee.IsDeleted)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "This account has been deleted.",
                    MustChangePassword = false
                };
            }

            // Check if account is inactive
            if (!employee.IsActive)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "This account is disabled.",
                    MustChangePassword = false
                };
            }
            // Revoke all previous refresh tokens
            employee.TokenVersion++;
            await _employeeRepository.UpdateAsync(employee);

            // Login successful
            var tokens = _jwtService.GenerateTokens(employee);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                MustChangePassword = employee.MustChangePassword,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessTokenExpiresAt
            };
        }
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(
    RefreshTokenRequestDto request)
        {

            //checking if there were already accesstoken-refresh token generated in 5 
            //(checking in memory cache.)
            var cached = _graceCache.Get(request.RefreshToken);

            if (cached != null)
            {
                return cached;
            }
            ClaimsPrincipal? principal;

            try
            {
                //taking the pricipal from the token passed
                principal = _jwtService.GetPrincipalFromToken(request.RefreshToken);
            }
            catch
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            if (principal == null)
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            var tokenType = principal.FindFirst("TokenType")?.Value;

            if (tokenType != "Refresh")
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid token type."
                };
            }

            var employeeCode = principal.FindFirst("EmployeeCode")?.Value;

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }


            var employee = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);

            if (employee == null)
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (!employee.IsActive || employee.IsDeleted)
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "User account is inactive."
                };
            }

            //taking the tokenversion from the refreshtoken
            var tokenVersion = int.Parse(
                principal.FindFirst("TokenVersion")!.Value);

            
            //checking if the tokenversion in refresh token is matching the one in the db
            if (tokenVersion != employee.TokenVersion)
            {
                return new RefreshTokenResponseDto
                {
                    Success = false,
                    Message = "Refresh token has been revoked."
                };
            }

            var shouldRotate =
                _jwtService.ShouldRotateRefreshToken(request.RefreshToken);

            if (shouldRotate)
            {
                employee.TokenVersion++;

                await _employeeRepository.UpdateAsync(employee);
            }

            var tokens = _jwtService.GenerateTokens(employee);
            
            //everything succeeds and proceeding to create tokens.
            var response =  new RefreshTokenResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully.",

                AccessToken = tokens.AccessToken,

                RefreshToken = shouldRotate
                    ? tokens.RefreshToken
                    : null,

                ExpiresAt = tokens.AccessTokenExpiresAt
            };

            //storing the tokens in cache to tackle the network race conditions
            _graceCache.Set(
            request.RefreshToken,
            response,
            TimeSpan.FromSeconds(5));

            return response;
        }
    }

}
