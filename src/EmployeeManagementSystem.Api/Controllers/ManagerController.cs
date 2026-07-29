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

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }



        //get all assigned employees(search sort available)
        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] EmployeeQueryParameters parameters)
        {
            var managerCode = User.FindFirst("EmployeeCode")?.Value;
            var response =  await _managerService.GetAssignedEmployeesAsync(managerCode, parameters);
            return Ok(response);
        }

        //get a specific employee(using employee code)
        [HttpGet("employees/{employeeCode}")]
        public async Task<IActionResult> GetAssignedEmployee(string employeeCode)
        {
            var managerCode = User.FindFirst("EmployeeCode")?.Value;
            var response = await _managerService.GetAssignedEmployeeAsync(managerCode, employeeCode);
            return Ok(response);
        }
    }
}
