using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EmployeeManagementSystem.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        //private readonly IRoleRepository _roleRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IEmployeeCodeGenerator _employeeCodeGenerator;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IEmployeeRepository employeeRepository, IPasswordService passwordService, IEmailService emailService, IAdminRepository adminRepository, IEmployeeCodeGenerator employeeCodeGenerator, ILogger<AdminService> logger)
        {
            _employeeRepository = employeeRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            //_roleRepository = roleRepository;
            _adminRepository = adminRepository;
            _employeeCodeGenerator = employeeCodeGenerator;
            _logger = logger;
        }

        //create employee
        public async Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            _logger.LogInformation("Creating employee {Email}",request.Email);


            var emailExists = await _employeeRepository.EmailExistsAsync(request.Email);
            
            if(request.Role == Role.Admin)
            {
                _logger.LogWarning("Employee cannot be created as the role specified is Admin.");
                throw new ConflictException("Role cannot be admin");
            }

            int? managerId = null;

            if (!string.IsNullOrWhiteSpace(request.ManagerEmployeeCode))
            {
                var manager = await _employeeRepository.GetByEmployeeCodeAsync(request.ManagerEmployeeCode);

                if (manager == null)
                { 
                    _logger.LogWarning("Employee creation failed because no manager exist at given manager code: {ManagerEmployeeCode}", request.ManagerEmployeeCode);
                    throw new NotFoundException("Manager not found."); 
                }

                if (!manager.IsActive || manager.IsDeleted)
                {
                    _logger.LogWarning("Employee creation failed because manager is either deleted or inactive. manager code: {ManagerEmployeeCode}", request.ManagerEmployeeCode);
                    throw new ConflictException("Selected manager is inactive.");
                
                }

                if (manager.Role != Role.Manager && manager.Role != Role.Admin)
                {
                    _logger.LogWarning("Employee creation failed because an employee cannot be a manager. manager code: {ManagerEmployeeCode}", request.ManagerEmployeeCode);
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
            

            employee.EmployeeCode = await _employeeCodeGenerator.GenerateEmployeeCodeAsync(request.Role);

            await _employeeRepository.UpdateAsync(employee);

            //successful creation
            _logger.LogInformation("Employee {EmployeeCode} created successfully.", employee.EmployeeCode);

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

        // get all employees including paging
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
            _logger.LogInformation("Changing status of employee {EmployeeCode}",employeeCode);
            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null) {

                _logger.LogWarning(
                    "Status update failed. Employee {EmployeeCode} not found.",
                     employeeCode);
                throw new NotFoundException("The user is not found!!!");
            }

            //trying to change his own staus
            if (!isActive && employee.EmployeeCode == currentEmployeeCode)
            {
                _logger.LogWarning("Employee {EmployeeCode} attempted to disable their own account.", currentEmployeeCode);
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

            await _adminRepository.SaveChangesAsync(employee);
            _logger.LogInformation(
                "Employee {EmployeeCode} status changed to {Status}",
                employee.EmployeeCode,
                employee.IsActive ? "Active" : "Inactive");
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


        // edit an employee
        public async Task<EmployeeDetailsResponseDto> UpdateEmployeeAsync(string employeeCode, UpdateEmployeeRequest request)
        {

            _logger.LogInformation("Updating employee {EmployeeCode}", employeeCode);
            
            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null)
            {
                _logger.LogWarning("Update failed. Employee {EmployeeCode} not found.", employeeCode);
                throw new NotFoundException("The Employee is not found, kindly check the Employee Code");
            }

            //admin cant change his role (only 1 admin and if role changes the system breaks)
            if (employee.Role == Role.Admin && request.Role != Role.Admin)
            {
                _logger.LogWarning("The only admin cannot be changed to another role.");
                throw new ConflictException("The only admin cannot be changed to another role.");
            }

            // check if the email is already present (excluding the current email)
            if (!employee.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _employeeRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("The email trying to change to {Email} already exists", request.Email);
                    throw new ConflictException("Email already exists.");
                }
            }

            if (request.Role == Role.Admin)
            {
                _logger.LogWarning("cant change an employee to admin");
                throw new ConflictException("Cant change an Employee to Admin");
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Email = request.Email;
            employee.Role = request.Role;

            employee.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(employee);

            _logger.LogInformation("Employee {EmployeeCode} updated successfully.", employeeCode);

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

        //delete an employee
        public async Task DeleteEmployeeAsync(string employeeCode, string currentEmployeeCode)
        {
            _logger.LogInformation("In delete to delete {employeeCode}", employeeCode);

            if (currentEmployeeCode == employeeCode)
            {
                _logger.LogWarning("Employee {EmployeeCode} attempted to delete their own account.", employeeCode);
                throw new ConflictException("You cant delete your own account!");
            }

            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null)
            {
                _logger.LogWarning("Delete failed because {employeeCode} doesnot exist", employeeCode);
                throw new NotFoundException("Employee not found. Please check the EmployeeCode");
            }

            if (employee.IsDeleted)
            {
                _logger.LogWarning("Delete failed. Employee {EmployeeCode} is already deleted.", employeeCode);
                throw new ConflictException("Employee is already deleted.");
            }
            if (employee.Role == Role.Admin)
            {
                _logger.LogWarning("Delete failed. Admin account {EmployeeCode} cannot be deleted.", employeeCode);
                throw new ConflictException("Admin account cannot be deleted.");
            }

            employee.IsDeleted = true;
            employee.IsActive = false;
            employee.TokenVersion++;
            employee.UpdatedAt = DateTime.UtcNow;



            await _adminRepository.SaveChangesAsync(employee);

            _logger.LogInformation("Employee {EmployeeCode} deleted",employee.EmployeeCode);
        }


        // change manager
        public async Task ChangeReportingManagerAsync(string employeeCode, string managerEmployeeCode)
        {
            _logger.LogInformation("Changing reporting manager for employee {EmployeeCode}",employeeCode);
            var employee = await _adminRepository.GetEmployeeByEmployeeCodeAsync(employeeCode);
            if (employee == null)
            {
                _logger.LogWarning("Change in RM failed, {EmployeeCode} does not exist", employeeCode);
                throw new NotFoundException("Employee not found please check the EmployeeCode");
            }
            
            var manager = await _adminRepository.GetEmployeeByEmployeeCodeAsync(managerEmployeeCode);
            
            
            if(manager == null)
            {
                _logger.LogWarning("Change in RM failed, {managerEmployeeCode} does not exist", managerEmployeeCode);
                throw new NotFoundException("Manager not found please check the EmployeeCode");
            }

            if (employeeCode == managerEmployeeCode)
            {
                _logger.LogWarning("Change in RM failed, Manager and employee is same");
                throw new ConflictException("Manager and employee has to be differnet");
            }

            if (manager.Role != Role.Admin && manager.Role != Role.Manager)
            {
                _logger.LogWarning("Change in RM failed, {managerEmployeeCode} is an employee and cannot be a manager", managerEmployeeCode);
                throw new ConflictException("This employee cannot be a manager.");
            }

            if (!manager.IsActive || manager.IsDeleted)
            {
                _logger.LogWarning("Change in RM failed, {managerEmployeeCode} is either deleted or inactive", managerEmployeeCode);
                throw new ConflictException("Selected manager is inactive.");
            }

            

            //if (employee.Role == Role.Manager && manager.Role != Role.Admin)
            //    throw new ConflictException("Manager can only have admin as manager");

            if (employee.Role == Role.Admin)
            {
                _logger.LogWarning("Change in RM failed, {EmployeeCode} is admin and admin cant have manager", employeeCode);
                throw new ConflictException("Admin cannot have a manager");
            }

            if (employee.ManagerId == manager.Id)
            {
                _logger.LogWarning("Change in RM failed, the employee was already assigned to the manager");
                throw new ConflictException("Employee is already assigned to this manager.");
            }

            employee.ManagerId = manager.Id;
            employee.UpdatedAt = DateTime.UtcNow;

            await _adminRepository.SaveChangesAsync(employee);
            _logger.LogInformation("Employee {EmployeeCode} assigned to manager {ManagerCode}",employee.EmployeeCode,manager.EmployeeCode);
        }


        //list deletedemployees
        public async Task<PagedResponse<EmployeeListDto>> GetDeletedEmployeesAsync(EmployeeQueryParameters parameters)
        {


            var (employees, totalCount) = await _adminRepository.GetDeletedEmployeesAsync(parameters);

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

        
    }
}
