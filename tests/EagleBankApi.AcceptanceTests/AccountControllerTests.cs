using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using EagleBank.Application.Models;
using FluentAssertions;

namespace EagleBankApi.AcceptanceTests;

public class AccountControllerTests(CustomWebApplicationFactory factory) : ControllerTestBase(factory)
{
    [Fact]
    public async Task GetAccount_ReturnsAccount_WhenExists()
    {
        // Arrange
        SetTestAuthToken(TestUserId);

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{TestAccountNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var account = await response.Content.ReadFromJsonAsync<AccountResponse>();
        account.Should().NotBeNull();
        account.AccountNumber.Should().Be(TestAccountNumber);
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreatedResponse_WithValidData()
    {
        // Arrange
        SetTestAuthToken(TestUserId);

        var request = _fixture.Create<CreateAccountRequest>();

        // Act
        var response = await _client.PostAsJsonAsync("/v1/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdAccount = await response.Content.ReadFromJsonAsync<AccountResponse>();
        createdAccount.Should().NotBeNull();
        createdAccount.Name.Should().Be(request.Name);
        createdAccount.AccountType.Should().Be(request.AccountType);
        createdAccount.Balance.Should().Be(0.00m);
    }

    [Fact]
    public async Task ListAccounts_ReturnsAllUserAccounts()
    {
        // Arrange
        SetTestAuthToken(TestUserId);

        // Act
        var response = await _client.GetAsync("/v1/accounts");

        // Assert
        response.EnsureSuccessStatusCode();

        var accounts = await response.Content.ReadFromJsonAsync<ListAccountsResponse>();
        accounts.Accounts.Should().HaveCount(1);
        accounts.Accounts.First().AccountNumber.Should().Be(TestAccountNumber);
    }

    [Fact]
    public async Task UpdateAccount_UpdatesAccountDetails()
    {
        // Arrange
        SetTestAuthToken(TestUserId);

        var updateRequest = _fixture.Create<UpdateAccountRequest>();

        // Act
        var response = await _client.PatchAsJsonAsync($"/v1/accounts/{TestAccountNumber}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedAccount = await response.Content.ReadFromJsonAsync<AccountResponse>();
        updatedAccount.Should().NotBeNull();
        updatedAccount.Name.Should().Be(updateRequest.Name);
        updatedAccount.AccountNumber.Should().Be(TestAccountNumber);
    }
}
