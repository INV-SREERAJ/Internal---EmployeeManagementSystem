using BCrypt.Net;
using EmployeeManagementSystem.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
