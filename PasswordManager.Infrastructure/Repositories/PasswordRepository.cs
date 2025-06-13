using Microsoft.EntityFrameworkCore;
using PasswordManager.Core.Interfaces;
using PasswordManager.Core.Models;
using PasswordManager.Infrastructure.Data;

namespace PasswordManager.Infrastructure.Repositories
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly PasswordManagerDbContext _context;

        public PasswordRepository(PasswordManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Password>> GetPasswordsByUserIdAsync(int userId)
        {
            return await _context.Passwords
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Password?> GetPasswordByIdAsync(int id, int userId)
        {
            return await _context.Passwords
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<Password> CreatePasswordAsync(Password password)
        {
            _context.Passwords.Add(password);
            await _context.SaveChangesAsync();
            return password;
        }

        public async Task<Password> UpdatePasswordAsync(Password password)
        {
            password.UpdatedAt = DateTime.UtcNow;
            _context.Passwords.Update(password);
            await _context.SaveChangesAsync();
            return password;
        }

        public async Task<bool> DeletePasswordAsync(int id, int userId)
        {
            var password = await GetPasswordByIdAsync(id, userId);
            if (password == null)
                return false;

            _context.Passwords.Remove(password);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}