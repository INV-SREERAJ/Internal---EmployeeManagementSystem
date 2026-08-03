using EmployeeManagementSystem.Business.DTOs.Auth;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IRefreshTokenGraceCache
    {
        RefreshTokenResponseDto? Get(string refreshToken);

        void Set(
            string refreshToken,
            RefreshTokenResponseDto response,
            TimeSpan duration);
    }
}