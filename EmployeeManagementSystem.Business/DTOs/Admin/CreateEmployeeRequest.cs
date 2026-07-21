
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Admin
{
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }

        public int? ManagerId { get; set; }
        public string? ManagerEmployeeCode { get; set; }

        public string PhoneNumber { get; set; }
    }
}
