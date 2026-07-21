using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RoleExistsById(int id)
        {
            return await _context.Roles.AnyAsync(x => x.Id==id);
        }
    }
}
