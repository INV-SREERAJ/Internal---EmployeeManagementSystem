
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IAdminRepository
    {
        Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(UserQueryParameters parameters);
    }
}
