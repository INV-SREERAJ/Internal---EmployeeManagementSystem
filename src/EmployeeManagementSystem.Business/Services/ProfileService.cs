using EmployeeManagementSystem.Business.DTOs.Profile;
using EmployeeManagementSystem.Business.DTOs.ProfileResponseDto;
using EmployeeManagementSystem.Business.GlobalExceptionHandler;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Interfaces;
using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class ProfileService : IProfileService
    {

        private readonly IProfileRepository _profileRepository;
        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        // get profile details
        public async Task<ProfileResponseDto> GetProfileAsync(ClaimsPrincipal user)
        {
            var employeeCode = user.FindFirst("EmployeeCode")?.Value;
            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                throw new UnAuthorizedException("Invalid User");
            }

            var employee = await _profileRepository.GetEmployeeAsync(employeeCode);
            if(employee == null)
            {
                throw new NotFoundException("Employee not found");
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


        //update details
        public async Task<ProfileResponseDto> UpdateProfileAsync(ClaimsPrincipal user, UpdateProfileRequestDto request)
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

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;

            await _profileRepository.SaveChangesAsync();

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
