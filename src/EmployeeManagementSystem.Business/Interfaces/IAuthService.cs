using EmployeeManagementSystem.Business.DTOs.Auth;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
