using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int id);

        Task UpdateAsync(User user);
    }
}
