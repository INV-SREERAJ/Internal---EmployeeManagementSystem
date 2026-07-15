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

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { 
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);  
        }
    }
}
