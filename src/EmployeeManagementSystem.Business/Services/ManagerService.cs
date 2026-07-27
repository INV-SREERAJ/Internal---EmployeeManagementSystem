using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;
using EmployeeManagementSystem.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(IManagerRepository managerRepository, IEmployeeRepository employeeRepository, ILogger<ManagerService> logger)
        {
            _managerRepository = managerRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<EmployeeDetailsResponseDto> GetAssignedEmployeeAsync(string managerCode, string employeeCode)
        {
            _logger.LogInformation("Getting an assigned employee {employeeCode} for manager {managerCode}", employeeCode, managerCode);
            var manager = await _employeeRepository.GetByEmployeeCodeAsync(managerCode);
            if(manager == null)
            {
                _logger.LogWarning("The given manager code is incorrect please check it : {managerCode}", managerCode);
                throw new NotFoundException("Manager not found!!!");
            }

            var employee = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);
            if(employee == null)
            {
                _logger.LogWarning("Getting employee for the manager failed as no employee exist for given employeeCOde : {employeeCode}", employeeCode);
                throw new NotFoundException("No employee found please check the code!!!");
            }

            var response = await _managerRepository.GetAssignedEmployeeAsync(manager.Id, employeeCode);
            if (response == null)
            {
                _logger.LogWarning(
                    "Employee {employeeCode} is not assigned to manager {managerCode}",
                    employeeCode,
                    managerCode);

                throw new NotFoundException("Employee not found.");
            }

            return new EmployeeDetailsResponseDto
            {
                EmployeeCode = response.EmployeeCode,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                PhoneNumber = response.PhoneNumber,
                Role = response.Role.ToString(),
                IsActive = response.IsActive,
                CreatedAt = response.CreatedAt,
                UpdatedAt = response.UpdatedAt
            };
        }

        public async Task<PagedResponse<EmployeeListDto>> GetAssignedEmployeesAsync(string managerCode, EmployeeQueryParameters employeeQueryParameters)
        {
            _logger.LogInformation(
                "Getting employees assigned under manager: {managerCode}",
                managerCode);

            var manager = await _employeeRepository.GetByEmployeeCodeAsync(managerCode);

            if (manager == null)
            {
                _logger.LogWarning(
                    "Getting employees failed. Manager with code {managerCode} was not found.",
                    managerCode);

                throw new NotFoundException("Manager not found.");
            }


            var (employees, totalCount) =
                await _managerRepository.GetAssignedEmployeesAsync(
                    manager.Id,
                    employeeQueryParameters);

            var employeeDtos = employees.Select(employee => new EmployeeListDto
            {
                EmployeeCode = employee.EmployeeCode,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Role = employee.Role.ToString(),
                ManagerName = employee.Manager == null
                    ? null
                    : $"{employee.Manager.FirstName} {employee.Manager.LastName}",
                IsActive = employee.IsActive
            });

            _logger.LogInformation(
                "Retrieved {Count} employees assigned to manager {managerCode}",
                totalCount,
                managerCode);

            return new PagedResponse<EmployeeListDto>
            {
                Data = employeeDtos,
                TotalCount = totalCount,
                PageNumber = employeeQueryParameters.PageNumber,
                PageSize = employeeQueryParameters.PageSize
            };
        }
    }
}
