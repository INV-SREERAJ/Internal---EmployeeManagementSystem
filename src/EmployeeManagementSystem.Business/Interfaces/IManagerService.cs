using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IManagerService
    {
        Task<PagedResponse<EmployeeListDto>> GetAssignedEmployeesAsync(string managerCode, EmployeeQueryParameters employeeQueryParameters);
        Task<EmployeeDetailsResponseDto> GetAssignedEmployeeAsync(string managerCode, string employeeCode);
    }
}
