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

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users.SingleOrDefaultAsync(x=>x.Email.Equals(email));
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedTimestamp = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(User user)
    {
        try
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("User can't be deleted if they have accounts");
        }
    }
}
