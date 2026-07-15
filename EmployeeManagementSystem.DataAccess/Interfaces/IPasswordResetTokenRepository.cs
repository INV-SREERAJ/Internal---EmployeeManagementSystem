using EmployeeManagementSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token);

        Task<PasswordResetToken?> GetValidTokenAsync(string token);

        Task UpdateAsync(PasswordResetToken token);
    }
}
