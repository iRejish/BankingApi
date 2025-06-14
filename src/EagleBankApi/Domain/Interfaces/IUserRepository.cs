using EagleBankApi.Domain.Entities;

namespace EagleBankApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(string userId);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(User user);
}
