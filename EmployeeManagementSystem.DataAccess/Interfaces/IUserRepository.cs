using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmployeeCodeAsync(string employeeCode);
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int id);

        Task UpdateAsync(User user);

        Task<bool> EmailExistsAsync(string email);

        Task<User> AddUserAsync(User user);
    }
}
