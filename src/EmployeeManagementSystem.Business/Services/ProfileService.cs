using EmployeeManagementSystem.Business.DTOs.Profile;
using EmployeeManagementSystem.Business.DTOs.ProfileResponseDto;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EmployeeManagementSystem.Business.Services
{
    public class ProfileService : IProfileService
    {
        //private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileService> _logger;
        private readonly IPasswordService _passwordService;
        private readonly IEmployeeRepository _employeeRepository;

        public ProfileService(ILogger<ProfileService> logger, IPasswordService passwordService, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _passwordService = passwordService;
            _employeeRepository = employeeRepository;
        }
        
        
        
        //change password
        public async Task ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequestDto request)
        {
            _logger.LogInformation("Changing password...");
            var employeeCode = user.FindFirst("EmployeeCode")?.Value;
            _logger.LogInformation("user : {employeeCode}", employeeCode);

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                _logger.LogWarning("Invalid employee code : {employeeCode}", employeeCode);
                throw new UnAuthorizedException("Invalid user.");
            }

            var employee = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);
            if(employee == null)
            {
                _logger.LogWarning("No employee found for employee code : {employeeCode}", employeeCode);
                throw new NotFoundException("user not found..");
            }

            var verification =  _passwordService.VerifyPassword(request.OldPassword, employee.PasswordHash);
            if (!verification)
            {
                _logger.LogWarning("Password change for user : {employeeCode} because the entered old password is incorrect.", employeeCode);
                throw new ConflictException("Invalid password.");
            }

            if(!(request.NewPassword .Equals(request.ConfirmPassword))) 
            {
                _logger.LogWarning("Change password failed due to mismatch in new password and confirm password");
                throw new ConflictException("Passwords doesnt match!");
            }

            if(_passwordService.VerifyPassword(request.NewPassword, employee.PasswordHash))
            {
                _logger.LogWarning("Password change failed, the old password and new password was same. employeeCode : {employeeCode}", employeeCode);
                throw new ConflictException("Old and new password cannot be the same..");
            }
            
            var passwordHash = _passwordService.HashPassword(request.NewPassword);
            employee.PasswordHash = passwordHash;
            employee.TokenVersion++;
            employee.MustChangePassword = false;
            await _employeeRepository.UpdateAsync(employee);

            _logger.LogInformation("Chnaged password for user: {employeeCode}, successfully.", employeeCode);

            
            
            
        }


        // Get profile details
        public async Task<ProfileResponseDto> GetProfileAsync(ClaimsPrincipal user)
        {
            var employeeCode = user.FindFirst("EmployeeCode")?.Value;

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                throw new UnAuthorizedException("Invalid user.");
            }

            var employee = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found.");
            }

            return new ProfileResponseDto
            {
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Role = employee.Role,
                ManagerCode = employee.Manager?.EmployeeCode,
                CreatedAt = employee.CreatedAt
            };
        }

        // Update profile
        public async Task<ProfileResponseDto> UpdateProfileAsync(
            ClaimsPrincipal user,
            UpdateProfileRequestDto request)
        {
            var employeeCode = user.FindFirst("EmployeeCode")?.Value;

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                throw new UnAuthorizedException("Invalid user.");
            }

            _logger.LogInformation(
                "Updating profile for employee {EmployeeCode}",
                employeeCode);

            var employee = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Profile update failed. Employee {EmployeeCode} not found.",
                    employeeCode);

                throw new NotFoundException("Employee not found.");
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;

            await _employeeRepository.UpdateAsync(employee);

            _logger.LogInformation(
                "Profile updated successfully for employee {EmployeeCode}",
                employee.EmployeeCode);

            return new ProfileResponseDto
            {
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Role = employee.Role,
                ManagerCode = employee.Manager?.EmployeeCode,
                CreatedAt = employee.CreatedAt
            };
        }
    }
}