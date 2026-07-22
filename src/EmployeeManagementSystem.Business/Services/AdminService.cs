using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        //private readonly IRoleRepository _roleRepository;
        private readonly IAdminRepository _adminRepository;
        public AdminService(IEmployeeRepository employeeRepository, IPasswordService passwordService, IEmailService emailService, IAdminRepository adminRepository)
        {
            _employeeRepository = employeeRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            //_roleRepository = roleRepository;
            _adminRepository = adminRepository;
        }
        public async Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var emailExists = await _employeeRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new ConflictException("Email already exists!!");
            }


            int? managerId = null;

            if (!string.IsNullOrWhiteSpace(request.ManagerEmployeeCode))
            {
                var manager = await _employeeRepository.GetByEmployeeCodeAsync(request.ManagerEmployeeCode);

                if (manager == null)
                    throw new NotFoundException("Manager not found.");

                if (!manager.IsActive || manager.IsDeleted)
                    throw new ConflictException("Selected manager is inactive.");

                if (manager.Role != Role.Manager && manager.Role != Role.Admin)
                {
                    throw new ConflictException("Selected employee cannot be assigned as manager.");
                }

                managerId = manager.Id;
            }



            var temporaryPassword = _passwordService.GenerateTemporaryPassword();

            var passwordHash = _passwordService.HashPassword(temporaryPassword);

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                ManagerId = managerId,

                PasswordHash = passwordHash,

                IsActive = true,
                IsDeleted = false,
                MustChangePassword = true,

                TokenVersion = 1,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _employeeRepository.AddEmployeeAsync(employee);
            employee.EmployeeCode = $"EMP{(employee.Id + 1120):D5}";
            await _employeeRepository.UpdateAsync(employee);
            await _emailService.WelcomeEmailAsync(
                employee.Email,
                $"{employee.FirstName} {employee.LastName}",
                temporaryPassword);
            return new CreateEmployeeResponse
            {
                EmployeeCode = employee.EmployeeCode,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Role = employee.Role.ToString(),
                Message = "User created successfully."
            };
        }

        public async Task<PagedResponse<EmployeeListDto>> GetEmployeesAsync(EmployeeQueryParameters parameters)
        {
            var (employees, totalCount) =
                await _adminRepository.GetEmployeesAsync(parameters);

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

            return new PagedResponse<EmployeeListDto>
            {
                Data = employeeDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
            };
        }

        public async Task<bool> UpdateEmployeeStatusAsync(string employeeCode, bool isActive, string currentEmployeeCode)
        {
            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null) {
                throw new NotFoundException("The user is not found!!!");
            }

            //trying to change his own staus
            if (!isActive && employee.EmployeeCode == currentEmployeeCode)
            {
                throw new ConflictException("You cannot disable your own account.");
            }

            //noting to change
            if (employee.IsActive == isActive)
                return false;


            employee.IsActive = isActive;
            employee.UpdatedAt = DateTime.UtcNow;
            //changing refresh token version to revoke access
            if (!isActive)
                employee.TokenVersion++;

            await _adminRepository.UpdateEmployeeAsync(employee);

            return true;
        }

        public async Task<EmployeeDetailsResponseDto> GetEmployeeDetailsAsync(string employeeCode)
        {
            var employee =  await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if(employee == null)
            {
                throw new NotFoundException("There is no such employee!");
            }
            return new EmployeeDetailsResponseDto
            {
                EmployeeCode = employeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Role = employee.Role.ToString(),
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        public async Task<EmployeeDetailsResponseDto> EditEmployeeAsync(string employeeCode, UpdateEmployeeRequest request)
        {
            
            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null)
                throw new NotFoundException("The Employee is not found, kindly check the Employee Code");

            //admin cant change his role (only 1 admin and if role changes the system breaks)
            if (employee.Role == Role.Admin && request.Role != Role.Admin)
            {
                throw new ConflictException("The only admin cannot be changed to another role.");
            }

            // check if the email is already present (excluding the current email)
            if (!employee.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _employeeRepository.EmailExistsAsync(request.Email))
                    throw new ConflictException("Email already exists.");
            }

            if (request.Role == Role.Admin)
                throw new ConflictException("Cant change an Employee to Admin");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Email = request.Email;
            employee.Role = request.Role;

            employee.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.UpdateEmployeeAsync(employee);

            return new EmployeeDetailsResponseDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email= employee.Email,
                EmployeeCode = employee.EmployeeCode,
                PhoneNumber= employee.PhoneNumber,
                Role= employee.Role.ToString(),
                IsActive= employee.IsActive,
                CreatedAt= employee.CreatedAt,
                UpdatedAt= employee.UpdatedAt
            };
        }

        public async Task DeleteEmployeeAsync(string employeeCode, string currentEmployeeCode)
        {

            if (currentEmployeeCode == employeeCode)
                throw new ConflictException("You cant delete your own account!");

            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null)
                throw new NotFoundException("Employee not found. Please check the EmployeeCode");

            if (employee.IsDeleted)
                throw new ConflictException("Employee is already deleted.");
            if (employee.Role == Role.Admin)
                throw new ConflictException("Admin account cannot be deleted.");

            employee.IsDeleted = true;
            employee.IsActive = false;
            employee.TokenVersion++;
            employee.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.UpdateEmployeeAsync(employee);

            return true;
        }
    }
}
