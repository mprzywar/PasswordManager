using Microsoft.EntityFrameworkCore;
using PasswordManager.Core.Interfaces;
using PasswordManager.Core.Models;
using PasswordManager.Infrastructure.Data;

namespace PasswordManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PasswordManagerDbContext _context;

        public UserRepository(PasswordManagerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }
    }
}