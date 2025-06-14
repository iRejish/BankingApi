using BankingApi.Services;
using EagleBankApi.Data.Entities;
using EagleBankApi.Models;
using EagleBankApi.Repositories;

namespace EagleBankApi.Services;

public class AccountService(IAccountRepository accountRepository, IUserRepository userRepository) : IAccountService
{
    public async Task<BankAccountResponse> CreateAccountAsync(CreateBankAccountRequest request, string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var account = new Account
        {
            AccountNumber = GenerateAccountNumber(),
            Name = request.Name,
            AccountType = request.AccountType,
            Balance = 0,
            UserId = userId
        };

        account = await accountRepository.CreateAsync(account);
        return MapToBankAccountResponse(account);
    }

    public async Task<ListBankAccountsResponse> ListAccountsAsync(string userId)
    {
        var accounts = await accountRepository.GetAllForUserAsync(userId);
        return new ListBankAccountsResponse
        {
            Accounts = accounts.Select(MapToBankAccountResponse).ToList()
        };
    }

    public async Task<BankAccountResponse> GetAccountByNumberAsync(string accountNumber, string userId)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        return MapToBankAccountResponse(account);
    }

    public async Task<BankAccountResponse> UpdateAccountAsync(string accountNumber, UpdateBankAccountRequest request, string userId)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        if (!string.IsNullOrEmpty(request.Name))
            account.Name = request.Name;

        if (request.AccountType != account.AccountType)
            account.AccountType = request.AccountType;

        account = await accountRepository.UpdateAsync(account);
        return MapToBankAccountResponse(account);
    }

    public async Task DeleteAccountAsync(string accountNumber, string userId)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        await accountRepository.DeleteAsync(account);
    }

    private static BankAccountResponse MapToBankAccountResponse(Account account)
    {
        return new BankAccountResponse
        {
            AccountNumber = account.AccountNumber,
            SortCode = account.SortCode,
            Name = account.Name,
            AccountType = account.AccountType,
            Balance = account.Balance,
            Currency = account.Currency,
            CreatedTimestamp = account.CreatedTimestamp,
            UpdatedTimestamp = account.UpdatedTimestamp
        };
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        return $"01{random.Next(100000, 999999)}";
    }
}
