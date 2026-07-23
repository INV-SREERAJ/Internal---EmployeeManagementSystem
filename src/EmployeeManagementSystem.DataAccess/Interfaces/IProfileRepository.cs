using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IProfileRepository
    {
        Task<Employee?> GetEmployeeAsync(string employeeCode);
        Task SaveChangesAsync();
    }
}
