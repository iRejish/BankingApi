using EagleBank.Application.Models;
using EagleBank.Application.Repositories;
using EagleBank.Domain.Entities;
using EagleBank.Domain.Exceptions;

namespace EagleBank.Application.Services;

public class TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
    : ITransactionService
{
    public async Task<TransactionResponse> CreateTransactionAsync(string userId,
        string accountNumber,
        CreateTransactionRequest request)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }

        if (account.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to access this account");
        }

        if (request.Type == "withdrawal" && account.Balance < request.Amount)
        {
            throw new FormatException("Account has insufficient funds");
        }

        var transaction = new Transaction
        {
            Amount = request.Amount,
            Currency = request.Currency,
            Type = request.Type,
            Reference = request.Reference,
            UserId = account.UserId,
            AccountNumber = accountNumber
        };

        transaction = await transactionRepository.CreateAsync(transaction);
        return MapToTransactionResponse(transaction);
    }

    public async Task<TransactionResponse> GetTransactionAsync(string userId,
        string accountNumber,
        string transactionId)
    {
        var transaction = await transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null || transaction.AccountNumber != accountNumber)
        {
            throw new KeyNotFoundException("Transaction not found");
        }

        if (transaction.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to access this transaction");
        }

        return MapToTransactionResponse(transaction);
    }

    public async Task<ListTransactionsResponse> ListTransactionsAsync(string userId, string accountNumber)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }

        if (account.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to access this account");
        }

        var transactions = await transactionRepository.GetByAccountNumberAsync(accountNumber);
        return new ListTransactionsResponse
        {
            Transactions = transactions.Select(MapToTransactionResponse)
        };
    }

    private static TransactionResponse MapToTransactionResponse(Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Type = transaction.Type,
            Reference = transaction.Reference,
            UserId = transaction.UserId,
            AccountNumber = transaction.AccountNumber,
            CreatedTimestamp = transaction.CreatedTimestamp
        };
    }
}
