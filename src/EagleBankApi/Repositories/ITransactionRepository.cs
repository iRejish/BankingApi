using EagleBankApi.Data.Entities;

namespace EagleBankApi.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(string transactionId);
    Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber);
}
