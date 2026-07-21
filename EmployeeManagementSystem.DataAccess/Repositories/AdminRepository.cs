using EmployeeManagementSystem.DataAccess.common;
using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Entities;
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

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(UserQueryParameters parameters)
        {
            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();

                query = query.Where(u =>
                    u.EmployeeCode.Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search));
            }

            // Filter by Role
            if (!string.IsNullOrWhiteSpace(parameters.Role))
            {
                query = query.Where(u => u.Role.Name == parameters.Role);
            }

            // Filter by Status
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == parameters.IsActive.Value);
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

            var users = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (users, totalCount);
        }
    }
}