
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IAdminRepository
    {
        Task<(IEnumerable<Employee> employees, int TotalCount)> GetEmployeesAsync(EmployeeQueryParameters parameters);

        Task<(IEnumerable<Employee> employees, int TotalCount)> GetDeletedEmployeesAsync(EmployeeQueryParameters parameters);

        Task<Employee?> GetEmployeeByEmployeeCodeAsync(string employeeCode);

        //enable/disable feature in admin
        Task UpdateEmployeeAsync(Employee employee);
    }
}
