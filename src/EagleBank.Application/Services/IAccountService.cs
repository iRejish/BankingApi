using EagleBank.Application.Models;

namespace EagleBank.Application.Services;

public interface IAccountService
{
    Task<AccountResponse> CreateAccountAsync(string userId, CreateAccountRequest request);
    Task<AccountResponse> GetAccountAsync(string userId, string accountNumber);
    Task<ListAccountsResponse> ListAccountsAsync(string userId);
    Task<AccountResponse> UpdateAccountAsync(string userId, string accountNumber, UpdateAccountRequest request);
    Task DeleteAccountAsync(string userId, string accountNumber);
}
