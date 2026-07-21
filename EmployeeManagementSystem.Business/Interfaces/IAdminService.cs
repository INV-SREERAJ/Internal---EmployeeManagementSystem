using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IAdminService
    {
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);

        Task<PagedResponse<UserListDto>> GetUsersAsync(UserQueryParameters parameters);
    }
}
