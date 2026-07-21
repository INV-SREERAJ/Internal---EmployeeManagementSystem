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
        Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request);

        Task<PagedResponse<EmployeeListDto>> GetEmployeesAsync(EmployeeQueryParameters parameters);
        Task<bool> UpdateEmployeeStatusAsync(string employeeCode, bool isActive, string currentEmployeeEmployeeCode);
    }
}
