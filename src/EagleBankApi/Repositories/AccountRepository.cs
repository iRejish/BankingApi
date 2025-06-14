using EagleBankApi.Data;
using EagleBankApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBankApi.Repositories;

public class AccountRepository(EagleBankDbContext context) : IAccountRepository
{
    public async Task<Account> CreateAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task<IEnumerable<Account>> GetAllForUserAsync(string userId)
    {
        return await context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, string userId)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.UserId == userId);
    }

    public async Task<Account> GetByAccountNumberAsync(string accountNumber)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(Account account)
    {
        context.Accounts.Remove(account);
        await context.SaveChangesAsync();
    }
}
