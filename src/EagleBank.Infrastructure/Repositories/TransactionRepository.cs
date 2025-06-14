using EagleBank.Application.Repositories;
using EagleBank.Domain.Entities;
using EagleBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Infrastructure.Repositories;

public class TransactionRepository(EagleBankDbContext context) : ITransactionRepository
{
    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        await using var dbTransaction = await context.Database.BeginTransactionAsync();
        try
        {
            var account = await context.Accounts.FirstAsync(a => a.AccountNumber == transaction.AccountNumber);
            account.Balance = transaction.Type switch
            {
                "deposit" => account.Balance + transaction.Amount,
                "withdrawal" => account.Balance - transaction.Amount,
                _ => throw new ArgumentException("Invalid transaction type")
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            return transaction;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Transaction> GetByIdAsync(string transactionId)
    {
        return await context.Transactions.FindAsync(transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber)
    {
        return await context.Transactions
            .Where(t => t.AccountNumber == accountNumber)
            .OrderByDescending(t => t.CreatedTimestamp)
            .ToListAsync();
    }
}
