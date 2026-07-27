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
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            IProfileRepository profileRepository,
            ILogger<ProfileService> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        // Get profile details
        public async Task<ProfileResponseDto> GetProfileAsync(ClaimsPrincipal user)
        {
            var employeeCode = user.FindFirst("EmployeeCode")?.Value;

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                throw new UnAuthorizedException("Invalid user.");
            }

            var employee = await _profileRepository.GetEmployeeAsync(employeeCode);

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

            var employee = await _profileRepository.GetEmployeeAsync(employeeCode);

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

            await _profileRepository.SaveChangesAsync();

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