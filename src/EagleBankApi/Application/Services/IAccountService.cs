using EagleBankApi.Application.Models;

namespace EagleBankApi.Application.Services;

public interface IAccountService
{
    Task<BankAccountResponse> CreateAccountAsync(CreateBankAccountRequest request, string userId);
    Task<ListBankAccountsResponse> ListAccountsAsync(string userId);
    Task<BankAccountResponse> GetAccountByNumberAsync(string accountNumber, string userId);
    Task<BankAccountResponse> UpdateAccountAsync(string accountNumber, UpdateBankAccountRequest request, string userId);
    Task DeleteAccountAsync(string accountNumber, string userId);
}
