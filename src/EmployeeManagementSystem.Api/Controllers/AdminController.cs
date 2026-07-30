using EmployeeManagementSystem.Business.DTOs.Admin;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
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

        //create an employee
        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var response = await _adminService.CreateEmployeeAsync(request);

            return CreatedAtAction(
                nameof(CreateEmployee),
                new { employeeCode = response.EmployeeCode },
                response);
        }

        //get all  available employees
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeQueryParameters parameters)
        {
            var result = await _adminService.GetEmployeesAsync(parameters);

            return Ok(result);
        }

        ////get deleted employees
        //[HttpGet("employees/deleted")]
        //public async Task<IActionResult> GetDeletedEmployees([FromQuery] EmployeeQueryParameters parameters)
        //{
        //    var result = await _adminService.GetDeletedEmployeesAsync(parameters);
        //    return Ok(result);
        //}

        //get employee by EmployeeCode
        [HttpGet("employees/{EmployeeCode}")]
        public async Task<IActionResult> GetEmployee([FromRoute] string EmployeeCode)
        {
            var employee = await _adminService.GetEmployeeDetailsAsync(EmployeeCode);
            return Ok(employee);
        }
        



        //update an employee
        [HttpPut("employees/{EmployeeCode}")]
        public async Task<IActionResult> EditEmployeeAsync([FromRoute] string EmployeeCode, [FromBody] UpdateEmployeeRequest request)
        {
            var employeeDetails = await _adminService.UpdateEmployeeAsync(EmployeeCode, request);
            return Ok(employeeDetails);
        }

        //change status
        [HttpPatch("employees/{EmployeeCode}/status")]
        public async Task<IActionResult> ChangeEmployeeStatus([FromBody] UpdateEmployeeStatusRequest statusRequest, [FromRoute] string EmployeeCode)
        {
            var currentEmployeeCode = User.FindFirst("EmployeeCode")?.Value;
            if (string.IsNullOrWhiteSpace(currentEmployeeCode))
                return Unauthorized();

            var statusChanged = await _adminService.UpdateEmployeeStatusAsync(EmployeeCode, statusRequest.Status, currentEmployeeCode);

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
                    Message = statusRequest.Status switch
                    {
                        EmployeeStatus.Active => "Employee activated successfully.",
                        EmployeeStatus.Inactive => "Employee deactivated successfully.",
                        EmployeeStatus.Deleted => "Employee deleted successfully.",
                        _ => "Employee status updated successfully."
                    }
                }
             );
        }



        //change reporting manager.
        [HttpPatch("employees/{EmployeeCode}/manager")]
        public async Task<IActionResult> ChangeReportingManager([FromRoute] string EmployeeCode, [FromBody] string managerEmployeeCode)
        {
            await _adminService.ChangeReportingManagerAsync(EmployeeCode, managerEmployeeCode);
            return Ok(
                new
                {
                    Message = "Reporting manager has been successfully changed"
                }
            );
        }



        ////soft delete an employee
        //[HttpDelete("employees/{EmployeeCode}")]
        //public async Task<IActionResult> DeleteEmployee([FromRoute]string EmployeeCode)
        //{
        //    var currEmployeeCode = User.FindFirst("EmployeeCode")?.Value;
        //    if (string.IsNullOrWhiteSpace(currEmployeeCode))
        //        return Unauthorized();


        //    await _adminService.DeleteEmployeeAsync(EmployeeCode, currEmployeeCode);
        //        return Ok(
        //            new
        //            {
        //                Message = "User Deleted Successfully!"
        //            }
        //            );
        //}



        //reset password and send email
        [HttpPost("employees/{employeeCode}")]
        public async Task<IActionResult> ResetPassword(string employeeCode)
        {
            await _adminService.ResetUserPasswordAsync(employeeCode);
            return Ok("The password for the user has been reset.");
        }


        


        
    }
}