using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);
            if (!loginResponse.Success)
                return Unauthorized(loginResponse);

            return Ok(loginResponse);

        }
    }
}
