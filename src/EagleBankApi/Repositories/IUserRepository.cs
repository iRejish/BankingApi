using EagleBankApi.Data.Entities;

namespace EagleBankApi.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(string userId);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(User user);
}
