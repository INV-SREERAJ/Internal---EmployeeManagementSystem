using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IManagerRepository
    {
        Task<(IEnumerable<Employee> employees, int TotalCount)> GetAssignedEmployeesAsync(int managerId, EmployeeQueryParameters parameters);
        Task<Employee?> GetAssignedEmployeeAsync(int managerId, string employeeCode);
        Task<bool> HasActiveDirectReportsAsync(int managerId);
    }
}
