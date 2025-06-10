using EagleBankApi.Data;
using EagleBankApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBankApi.Repositories;

public class UserRepository(EagleBankDbContext context) : IUserRepository
{
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        return await context.Users.FindAsync(userId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email.ToLower());
    }
}
