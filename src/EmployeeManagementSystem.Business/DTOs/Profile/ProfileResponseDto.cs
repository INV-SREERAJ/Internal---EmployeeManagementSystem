using EmployeeManagementSystem.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.ProfileResponseDto
{
    public class ProfileResponseDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string? ManagerCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
