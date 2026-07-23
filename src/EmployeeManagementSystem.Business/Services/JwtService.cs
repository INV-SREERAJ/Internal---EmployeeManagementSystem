using EmployeeManagementSystem.Business.Configuration;
using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.DTOs.EmployeeManagementSystem.Business.DTOs.Authentication;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        

        //public string GenerateRefreshToken(Employee employee)
        //{
        //    throw new NotImplementedException();
        //}

        public TokenResponseDto GenerateTokenPair(Employee employee)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);


            return new TokenResponseDto
            {
                AccessToken = GenerateAccessToken(employee, accessTokenExpiry),
                RefreshToken = GenerateRefreshToken(employee, refreshTokenExpiry),
                AccessTokenExpiresAt = accessTokenExpiry
            };
        }

        //public TokenResponseDto GenerateTokens(Employee employee)
        //{
        //    var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
        //    var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        //    var accessToken = GenerateAccessToken(employee, accessTokenExpiry);
        //    var refreshToken = GenerateRefreshToken(employee, refreshTokenExpiry);

        //    return new TokenResponseDto
        //    {
        //        AccessToken = accessToken,
        //        RefreshToken = refreshToken,
        //        AccessTokenExpiresAt = accessTokenExpiry,
        //        RefreshTokenExpiresAt = refreshTokenExpiry
        //    };
        //}

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),

                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out _);
        }

        public bool ShouldRotateRefreshToken(string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(refreshToken);

            var expiresAt = jwt.ValidTo;

            return expiresAt <= DateTime.UtcNow.AddHours(24);
        }

        public string GenerateAccessToken(Employee employee, DateTime expiresAt)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.EmployeeCode),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role.ToString()),
                new Claim("EmployeeCode", employee.EmployeeCode),
                new Claim("TokenVersion", employee.TokenVersion.ToString()),
                new Claim("TokenType", "Access"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(Employee employee, DateTime expiresAt)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.EmployeeCode),
                new Claim("EmployeeCode", employee.EmployeeCode),
                new Claim("TokenVersion", employee.TokenVersion.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("TokenType", "Refresh")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public TokenResponseDto GenerateAccessTokenOnly(Employee employee)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);

            return new TokenResponseDto
            {
                AccessToken = GenerateAccessToken(employee, accessTokenExpiry),
                RefreshToken = null,
                AccessTokenExpiresAt = accessTokenExpiry
            };
        }
    }
}