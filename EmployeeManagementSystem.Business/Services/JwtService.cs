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

        public TokenResponseDto GenerateTokens(User user)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            var accessToken = GenerateAccessToken(user, accessTokenExpiry);
            var refreshToken = GenerateRefreshToken(user, refreshTokenExpiry);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = refreshTokenExpiry
            };
        }

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

        private string GenerateAccessToken(User user, DateTime expiresAt)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("TokenVersion", user.TokenVersion.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("TokenType", "Access")
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

        private string GenerateRefreshToken(User user, DateTime expiresAt)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("TokenVersion", user.TokenVersion.ToString()),
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

        //public string GenerateAccessToken(User user)
        //{
        //    ...
        //}

        //public string GenerateRefreshToken(User user)
        //{
        //    ...
        //}

        //public DateTime GetAccessTokenExpiry()
        //{
        //    ...
        //}

        //public DateTime GetRefreshTokenExpiration()
        //{
        //    ...
        //}
    }
}