using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsById(int id);
    }
}
