using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Admin
{
    public class EmployeeListDto
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? ManagerName { get; set; }

        public bool IsActive { get; set; }
    }
}
