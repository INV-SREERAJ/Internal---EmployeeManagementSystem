using EmployeeManagementSystem.Business.DTOs.Profile;
using EmployeeManagementSystem.Business.DTOs.ProfileResponseDto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileResponseDto> GetProfileAsync(ClaimsPrincipal user);
        Task<ProfileResponseDto> UpdateProfileAsync(ClaimsPrincipal user, UpdateProfileRequestDto request);
        Task ChangePasswordAsync(ClaimsPrincipal user,ChangePasswordRequestDto request);
    }
}
