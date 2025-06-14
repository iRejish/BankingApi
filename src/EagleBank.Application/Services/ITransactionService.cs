using EagleBank.Application.Models;

namespace EagleBank.Application.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateTransactionAsync(string userId, string accountNumber, CreateTransactionRequest request);
    Task<TransactionResponse> GetTransactionAsync(string userId, string accountNumber, string transactionId);
    Task<ListTransactionsResponse> ListTransactionsAsync(string userId, string accountNumber);
}
