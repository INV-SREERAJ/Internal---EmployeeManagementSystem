using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Interfaces
{
    public interface IEmailService
    {
        Task WelcomeEmailAsync(string email, string FullName, string tempPassword);
        Task ResetPasswordEmailAsync(string email, string fullName, string tempPassword);
    }
}
