using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Entities.Enums;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        

        private async Task<(IEnumerable<Employee> employees, int TotalCount)> GetEmployeesInternalAsync(EmployeeQueryParameters parameters, bool isDeleted)
        {
            var query = _context.Employees
                .Where(e => e.IsDeleted == isDeleted)
                .Include(e => e.Manager)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();

                query = query.Where(e =>
                    e.EmployeeCode.Contains(search) ||
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));
            }

            // Filter by Role
            if (!string.IsNullOrWhiteSpace(parameters.Role) && Enum.TryParse<Role>(parameters.Role, true, out var role))
            {
                query = query.Where(e => e.Role == role);
            }

            // Filter by Status
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == parameters.IsActive.Value);
            }

            // Sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "name" => parameters.Descending
                    ? query.OrderByDescending(u => u.FirstName)
                           .ThenByDescending(u => u.LastName)
                    : query.OrderBy(u => u.FirstName)
                           .ThenBy(u => u.LastName),

                "email" => parameters.Descending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                "employeecode" => parameters.Descending
                    ? query.OrderByDescending(u => u.EmployeeCode)
                    : query.OrderBy(u => u.EmployeeCode),

                "createdat" => parameters.Descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),

                _ => query.OrderBy(u => u.Id)
            };

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (employees, totalCount);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<Employee?> GetEmployeeByEmployeeCodeAsync(string employeeCode)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(u =>
                    u.EmployeeCode == employeeCode &&
                    !u.IsDeleted);
        }


        //get all available employees
        public Task<(IEnumerable<Employee> employees, int TotalCount)> GetEmployeesAsync(EmployeeQueryParameters parameters)
        {
            return GetEmployeesInternalAsync(parameters, false);
        }

        //list delted employees
        public Task<(IEnumerable<Employee> employees, int TotalCount)> GetDeletedEmployeesAsync(EmployeeQueryParameters parameters)
        {
            return GetEmployeesInternalAsync(parameters, true);
        }

        public async Task<string?> GetLastEmployeeCodeAsync(string prefix, int year)
        {
            string pattern = $"{prefix}{year}";

            return await _context.Employees
                .Where(e => e.EmployeeCode.StartsWith(pattern))
                .OrderByDescending(e => e.EmployeeCode)
                .Select(e => e.EmployeeCode)
                .FirstOrDefaultAsync();
        }
    }
}