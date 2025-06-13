using PasswordManager.Core.Models;

namespace PasswordManager.Core.Interfaces
{
    public interface IPasswordRepository
    {
        Task<List<Password>> GetPasswordsByUserIdAsync(int userId);
        Task<Password?> GetPasswordByIdAsync(int id, int userId);
        Task<Password> CreatePasswordAsync(Password password);
        Task<Password> UpdatePasswordAsync(Password password);
        Task<bool> DeletePasswordAsync(int id, int userId);
    }
}