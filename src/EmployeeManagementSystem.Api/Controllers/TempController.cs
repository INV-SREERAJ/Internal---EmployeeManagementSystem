using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private IAuthService _authService;

    public TestController(IAuthService authService)
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