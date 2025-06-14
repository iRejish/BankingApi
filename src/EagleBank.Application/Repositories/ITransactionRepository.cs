using EagleBank.Domain.Entities;

namespace EagleBank.Application.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> GetByIdAsync(string transactionId);
    Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber);
}
