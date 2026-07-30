using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/manager")]
    [Authorize(Roles = "Admin,Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(IManagerService managerService, ILogger<ManagerController> logger)
        {
            _managerService = managerService;
            _logger = logger;
        }



        //get all assigned employees(search sort available)
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] EmployeeQueryParameters parameters)
        {
            _logger.LogInformation("Inside GetAllEmployees in ManagerController");
            var managerCode = User.FindFirst("EmployeeCode")?.Value;
            var response =  await _managerService.GetAssignedEmployeesAsync(managerCode, parameters);
            return Ok(response);
        }

        //get a specific employee(using employee code)
        [HttpGet("employees/{employeeCode}")]
        public async Task<IActionResult> GetAssignedEmployee(string employeeCode)
        {
            _logger.LogInformation("Inside GetAssignedEmployee in ManagerController");
            var managerCode = User.FindFirst("EmployeeCode")?.Value;
            var response = await _managerService.GetAssignedEmployeeAsync(managerCode, employeeCode);
            return Ok(response);
        }
    }
}
