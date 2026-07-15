using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Auth
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
