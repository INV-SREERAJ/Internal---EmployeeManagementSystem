using EmployeeManagementSystem.Business.Common;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;

namespace EmployeeManagementSystem.Business.Services
{
    public class EmployeeCodeGenerator : IEmployeeCodeGenerator
    {
        private readonly IAdminRepository _adminRepository;

        public EmployeeCodeGenerator(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }


        // employee code of the form EMP20260001
        public async Task<Result<string>> GenerateEmployeeCodeAsync(Role role)
        {
            string prefix = "EMP";

            int year = DateTime.UtcNow.Year;

            string? lastCode =
                await _adminRepository.GetLastEmployeeCodeAsync(prefix, year);

            int nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastCode))
            {
                string sequence = lastCode.Substring(7);

                nextNumber = int.Parse(sequence) + 1;
            }

            return Result<string>.Ok($"{prefix}{year}{nextNumber:D4}");
        }
    }
}
