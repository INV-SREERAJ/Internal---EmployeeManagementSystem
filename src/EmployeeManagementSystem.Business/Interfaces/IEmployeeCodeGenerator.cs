using EmployeeManagementSystem.DataAccess.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IEmployeeCodeGenerator
    {
        Task<string> GenerateEmployeeCodeAsync(Role role);
    }
}
