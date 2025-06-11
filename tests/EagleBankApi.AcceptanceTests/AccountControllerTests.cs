using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using EagleBankApi.Models;

namespace EagleBankApi.AcceptanceTests;

public class AccountControllerTests(CustomWebApplicationFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task CreateAccount_ReturnsCreatedResponse_WithValidData()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);

        var request = _fixture.Create<CreateBankAccountRequest>();

        // Act

        var response = await _client.PostAsJsonAsync("/v1/accounts", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAccount = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
        Assert.Equal(request.Name, createdAccount.Name);
        Assert.Equal(request.AccountType, createdAccount.AccountType);
        Assert.Equal(0.00m, createdAccount.Balance);
    }

    [Fact]
    public async Task GetAccount_ReturnsAccount_WhenExists()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);
        var request = _fixture.Create<CreateBankAccountRequest>();
        var accountResponse = await _client.PostAsJsonAsync("/v1/accounts", request);
        var validAccount = await accountResponse.Content.ReadFromJsonAsync<BankAccountResponse>();

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{validAccount.AccountNumber}");

        // Assert
        response.EnsureSuccessStatusCode();
        var account = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
        Assert.Equal(validAccount.AccountNumber, account.AccountNumber);
        Assert.Equal(validAccount.Balance, account.Balance);
    }

    [Fact]
    public async Task ListAccounts_ReturnsAllUserAccounts()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);
        var request = _fixture.Create<CreateBankAccountRequest>();
        var accountResponse = await _client.PostAsJsonAsync("/v1/accounts", request);
        var validAccount = await accountResponse.Content.ReadFromJsonAsync<BankAccountResponse>();

        // Act
        var response = await _client.GetAsync("/v1/accounts");

        // Assert
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<ListBankAccountsResponse>();
        Assert.Single(accounts.Accounts);
        Assert.Equal(validAccount.AccountNumber, accounts.Accounts[0].AccountNumber);
    }

    [Fact]
    public async Task UpdateAccount_UpdatesAccountDetails()
    {
        // Arrange
        var user = await CreateTestUser();
        AuthenticateClient(user.Id);
        var request = _fixture.Create<CreateBankAccountRequest>();
        var accountResponse = await _client.PostAsJsonAsync("/v1/accounts", request);
        var validAccount = await accountResponse.Content.ReadFromJsonAsync<BankAccountResponse>();

        var updateRequest = new UpdateBankAccountRequest
        {
            Name = "Updated Account Name"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/v1/accounts/{validAccount.AccountNumber}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedAccount = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
        Assert.Equal("Updated Account Name", updatedAccount.Name);
    }
}
