using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private IAuthService _authService;
    private readonly IEmailService _emailService;

    public TestController(IAuthService authService, IEmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequest)
    {
        var loginResponse = await _authService.LoginAsync(loginRequest);
        if (!loginResponse.Success)
            return Unauthorized(loginResponse);

        return Ok(loginResponse);
        
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok("Protected endpoint");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(
    RefreshTokenRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendTestEmail()
    {
        await _emailService.WelcomeEmailAsync(
            "sreerajr342@gmail.com",
            "Sreeraj",
            "Temp@123");

        return Ok(new
        {
            Message = "Test email sent successfully."
        });
    }
}