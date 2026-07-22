using EmployeeManagementSystem.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Admin
{
    public class UpdateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public Role Role { get; set; }
    }
}
