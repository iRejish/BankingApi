using EagleBankApi.Domain.Entities;

namespace EagleBankApi.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(string transactionId);
    Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber);
}
