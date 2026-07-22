using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Auth
{
    public class RefreshTokenResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
