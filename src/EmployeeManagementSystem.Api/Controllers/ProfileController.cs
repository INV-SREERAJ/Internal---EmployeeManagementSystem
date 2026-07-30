using EmployeeManagementSystem.Business.DTOs.Profile;
using EmployeeManagementSystem.Business.Interfaces;
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
        private readonly ILogger _logger;

        public ProfileController(IProfileService profileService, ILogger logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        //view profile details
        [HttpGet]
        public async Task<IActionResult> GetProfileDetails()
        {
            _logger.LogInformation("Inside GetProfileDetails in ProfileController.");
            var result = await _profileService.GetProfileAsync(User);
            return Ok(result);
        }

        //update profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequestDto request)
        {
            _logger.LogInformation("Inside UpdateProfile in ProfileController.");
            var result = await _profileService.UpdateProfileAsync(User, request);
            return Ok(result);
        }


        //change password by entering old password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            _logger.LogInformation("Inside ChangePassword in ProfileController.");
            await _profileService.ChangePasswordAsync(User, request);
            return Ok(new
            {
                Message = "Password changed successfully."
            });
        }
    }
}
