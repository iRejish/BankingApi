using BankingApi.Repositories;
using EagleBankApi.Data;
using EagleBankApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleBankApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly EagleBankDbContext _context;

    public AccountRepository(EagleBankDbContext context)
    {
        _context = context;
    }

    public async Task<Account> CreateAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<IEnumerable<Account>> GetAllForUserAsync(string userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, string userId)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.UserId == userId);
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        account.UpdatedTimestamp = DateTime.UtcNow;
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(Account account)
    {
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}
