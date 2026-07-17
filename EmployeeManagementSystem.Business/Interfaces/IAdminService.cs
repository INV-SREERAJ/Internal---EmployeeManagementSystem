using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IAdminService
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    }
}
