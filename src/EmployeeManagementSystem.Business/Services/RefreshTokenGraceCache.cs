using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace EmployeeManagementSystem.Business.Services
{
    public class RefreshTokenGraceCache : IRefreshTokenGraceCache
    {
        private readonly IMemoryCache _cache;

        public RefreshTokenGraceCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public RefreshTokenResponseDto? Get(string refreshToken)
        {
            _cache.TryGetValue(refreshToken, out RefreshTokenResponseDto? response);

            return response;
        }

        public void Set(
            string refreshToken,
            RefreshTokenResponseDto response,
            TimeSpan duration)
            {
                _cache.Set(refreshToken, response, duration);
            }
    }
}