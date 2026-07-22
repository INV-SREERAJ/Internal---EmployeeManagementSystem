using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByEmployeeCodeAsync(string employeeCode);
        Task<Employee?> GetByEmailAsync(string email);

        Task<Employee?> GetByIdAsync(int id);

        Task UpdateAsync(Employee employee);

        Task<bool> EmailExistsAsync(string email);

        Task<Employee> AddEmployeeAsync(Employee employee);
    }
}
