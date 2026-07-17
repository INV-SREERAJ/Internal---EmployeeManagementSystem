using EmployeeManagementSystem.Business.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
