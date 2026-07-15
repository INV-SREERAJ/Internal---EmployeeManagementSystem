using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
