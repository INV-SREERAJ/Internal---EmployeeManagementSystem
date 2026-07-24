using EmployeeManagementSystem.Business.DTOs.EmployeeManagementSystem.Business.DTOs.Authentication;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IJwtService
    {

        ClaimsPrincipal? GetPrincipalFromToken(string token);
        string GenerateAccessToken(Employee employee, DateTime expiresAt);
        string GenerateRefreshToken(Employee employee, DateTime expiresAt);
        TokenResponseDto GenerateTokenPair(Employee employee);
        bool ShouldRotateRefreshToken(string refreshToken);
        TokenResponseDto GenerateAccessTokenOnly(Employee employee);
    }
}
