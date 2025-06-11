using EagleBankApi.Data.Entities;

namespace BankingApi.Repositories;

public interface IAccountRepository
{
    Task<Account> CreateAsync(Account account);
    Task<IEnumerable<Account>> GetAllForUserAsync(string userId);
    Task<Account?> GetByAccountNumberAsync(string accountNumber, string userId);
    Task<Account> UpdateAsync(Account account);
    Task DeleteAsync(Account account);
}
