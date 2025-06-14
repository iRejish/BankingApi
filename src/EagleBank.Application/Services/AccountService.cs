using EagleBank.Application.Models;
using EagleBank.Application.Repositories;
using EagleBank.Domain.Entities;

namespace EagleBank.Application.Services;

public class AccountService(IAccountRepository accountRepository, IUserRepository userRepository) : IAccountService
{
    public async Task<AccountResponse> CreateAccountAsync(string userId, CreateAccountRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var account = new Account
        {
            AccountNumber = GenerateAccountNumber(),
            Name = request.Name,
            AccountType = request.AccountType,
            UserId = userId
        };

        var createdAccount = await accountRepository.CreateAsync(account);
        return MapToResponse(createdAccount);
    }

    public async Task<AccountResponse> GetAccountAsync(string userId, string accountNumber)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        return MapToResponse(account);
    }

    public async Task<ListAccountsResponse> ListAccountsAsync(string userId)
    {
        var accounts = await accountRepository.GetAllForUserAsync(userId);
        return new ListAccountsResponse
        {
            Accounts = accounts.Select(MapToResponse)
        };
    }

    public async Task<AccountResponse> UpdateAccountAsync(string userId, string accountNumber, UpdateAccountRequest request)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        account.Name = request.Name;
        account.UpdatedTimestamp = DateTime.UtcNow;

        var updatedAccount = await accountRepository.UpdateAsync(account);
        return MapToResponse(updatedAccount);
    }

    public async Task DeleteAccountAsync(string userId, string accountNumber)
    {
        var account = await accountRepository.GetByAccountNumberAsync(accountNumber, userId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");

        await accountRepository.DeleteAsync(account);
    }

    private static string GenerateAccountNumber()
    {
        return $"01{Random.Shared.Next(100000, 999999)}";
    }

    private static AccountResponse MapToResponse(Account account)
    {
        return new AccountResponse
        {
            AccountNumber = account.AccountNumber,
            SortCode = account.SortCode,
            Name = account.Name,
            AccountType = account.AccountType,
            Balance = account.Balance,
            Currency = account.Currency,
            UserId = account.UserId,
            CreatedTimestamp = account.CreatedTimestamp,
            UpdatedTimestamp = account.UpdatedTimestamp
        };
    }
}
