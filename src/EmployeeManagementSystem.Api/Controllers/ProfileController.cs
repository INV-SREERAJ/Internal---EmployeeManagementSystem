using EmployeeManagementSystem.Business.DTOs.Profile;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        //view profile details
        [HttpGet]
        public async Task<IActionResult> GetProfileDetails()
        {
            var result = await _profileService.GetProfileAsync(User);
            return Ok(result);
        }

        //update profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequestDto request)
        {
            var result = await _profileService.UpdateProfileAsync(User, request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            await _profileService.ChangePasswordAsync(User, request);
            return Ok(new
            {
                Message = "Password changed successfully."
            });
        }
    }
}
