using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Auth
{
    public class LoginResponseDto
    {   
        //for future
        //public string AccessToken { get; set; }
        //public string RefreshToken { get; set; }
        //public DateTime ExpiresAt { get; set; }

        public bool MustChangePassword { get; set; }

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
