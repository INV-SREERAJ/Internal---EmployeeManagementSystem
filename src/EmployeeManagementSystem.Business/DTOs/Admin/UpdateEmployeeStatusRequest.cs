using EmployeeManagementSystem.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.DTOs.Admin
{
    public class UpdateEmployeeStatusRequest
    {
        public EmployeeStatus Status { get; set; }
    }
}
