using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.Services
{
    public class EmployeeCodeGenerator : IEmployeeCodeGenerator
    {
        private readonly IAdminRepository _adminRepository;

        public EmployeeCodeGenerator(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<string> GenerateEmployeeCodeAsync(Role role)
        {
            string prefix = role switch
            {
                Role.Admin => "ADM",
                Role.Manager => "MNG",
                Role.Employee => "EMP",
                _ => throw new ArgumentException("Invalid role")
            };

            int year = DateTime.UtcNow.Year;

            string? lastCode =
                await _adminRepository.GetLastEmployeeCodeAsync(prefix, year);

            int nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastCode))
            {
                string sequence = lastCode.Substring(7);

                nextNumber = int.Parse(sequence) + 1;
            }

            return $"{prefix}{year}{nextNumber:D4}";
        }
    }
}
