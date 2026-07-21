using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var response = await _adminService.CreateEmployeeAsync(request);

            return CreatedAtAction(
                nameof(CreateEmployee),
                new { employeeCode = response.EmployeeCode },
                response);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeQueryParameters parameters)
        {
            var result = await _adminService.GetEmployeesAsync(parameters);

            return Ok(result);
        }
    }
}