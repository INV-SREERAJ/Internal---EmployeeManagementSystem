using EmployeeManagementSystem.DataAccess.Context;
using EmployeeManagementSystem.DataAccess.Entities;
using EmployeeManagementSystem.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == token &&
                    !x.IsUsed &&
                    x.ExpiryTime > DateTime.UtcNow);
        }

        public async Task UpdateAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}