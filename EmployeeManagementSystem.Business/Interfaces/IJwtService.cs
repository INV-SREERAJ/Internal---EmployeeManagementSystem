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
        TokenResponseDto GenerateTokens(User user);
        bool ShouldRotateRefreshToken(string refreshToken);
    }
}
