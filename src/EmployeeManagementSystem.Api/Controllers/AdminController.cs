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

        [HttpPatch("employees/{EmployeeCode}/status")]
        public async Task<IActionResult> ChangeEmployeeStatus([FromBody] UpdateEmployeeStatusRequest statusRequest, [FromRoute] string EmployeeCode)
        {
            var currentEmployeeCode = User.FindFirst("EmployeeCode")?.Value;
            if (string.IsNullOrWhiteSpace(currentEmployeeCode))
                return Unauthorized();

            var statusChanged = await _adminService.UpdateEmployeeStatusAsync(EmployeeCode, statusRequest.IsActive, currentEmployeeCode);

            if (!statusChanged)
            {
                return Ok(new
                {
                    Message = "The Status is already up to date"
                }
                );
            }

            return Ok(
                new
                {
                    Message = statusRequest.IsActive
                    ? "Employee enabled successfully."
                    : "Employee disabled successfully."
                }
             );
        }
    }
}