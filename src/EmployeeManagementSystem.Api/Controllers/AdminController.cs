using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("/api/admin")]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await _adminService.CreateUserAsync(request);
            return CreatedAtAction(
            nameof(CreateUser),
            new { employeeCode = response.EmplooyeeCode},
            response);
        }
    }
}
