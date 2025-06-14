using System.Net;
using System.Net.Http.Json;
using EagleBankApi.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
using EagleBankApi.Data.Entities;

namespace EagleBankApi.AcceptanceTests;

public class TransactionControllerTests(CustomWebApplicationFactory factory) : ControllerTestBase(factory)
{
    private const string TestTransactionId = "tan-test123";

    [Fact]
    public async Task CreateDeposit_ReturnsCreatedResponse_WhenValid()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "deposit")
            .With(x => x.Amount, 100.50m)
            .With(x => x.Currency, "GBP")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{TestAccountNumber}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);


        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        transaction.Should().NotBeNull();
        transaction.Type.Should().Be("deposit");
        transaction.Amount.Should().Be(100.50m);

        // Verify balance updated
        var account = await _dbContext.Accounts.FirstAsync(a => a.AccountNumber == TestAccountNumber);
        await _dbContext.Entry(account).ReloadAsync(); // Ensure latest state from DB
        account.Balance.Should().Be(1100.50m); // initial balance was 1000
    }

    [Fact]
    public async Task CreateWithdrawal_ReturnsCreatedResponse_WhenSufficientFunds()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "withdrawal")
            .With(x => x.Amount, 100.00m)
            .With(x => x.Currency, "GBP")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{TestAccountNumber}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var transaction = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        transaction.Should().NotBeNull();
        transaction.Type.Should().Be("withdrawal");

        // Verify balance updated
        var account = await _dbContext.Accounts.FirstAsync(a => a.AccountNumber == TestAccountNumber);
        await _dbContext.Entry(account).ReloadAsync(); // Ensure latest state from DB
        account.Balance.Should().Be(900.00m); // initial balance was 1000
    }

    [Fact]
    public async Task CreateWithdrawal_ReturnsUnprocessableEntity_WhenInsufficientFunds()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "withdrawal")
            .With(x => x.Amount, 1500.00m) // More than balance
            .With(x => x.Currency, "GBP")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{TestAccountNumber}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsForbidden_ForOtherUsersAccount()
    {
        // Arrange
        SetTestAuthToken("usr-otheruser");
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "deposit")
            .With(x => x.Amount, 100.00m)
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{TestAccountNumber}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsNotFound_ForInvalidAccount()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "deposit")
            .Create();
        var nonExistingAccount = "01999999";

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{nonExistingAccount}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsBadRequest_ForInvalidData()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var request = _fixture.Build<CreateTransactionRequest>()
            .With(x => x.Type, "invalid")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/accounts/{TestAccountNumber}/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListTransactions_ReturnsTransactions_ForOwnAccount()
    {
        // Arrange
        SetTestAuthToken(TestUserId);

        // Seed test transactions
        _dbContext.Transactions.AddRange(
            _fixture.Build<Transaction>()
                .With(t => t.AccountNumber, TestAccountNumber)
                .With(t => t.Type, "deposit")
                .CreateMany(3));
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{TestAccountNumber}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var transactions = await response.Content.ReadFromJsonAsync<ListTransactionsResponse>();
        transactions.Transactions.Should().HaveCount(3);
    }

    [Fact]
    public async Task ListTransactions_ReturnsForbidden_ForOtherUsersAccount()
    {
        // Arrange
        SetTestAuthToken("usr-otheruser");

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{TestAccountNumber}/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTransaction_ReturnsTransaction_WhenExists()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var transaction = _fixture.Build<Transaction>()
            .With(t => t.Id, TestTransactionId)
            .With(t => t.AccountNumber, TestAccountNumber)
            .Create();
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{TestAccountNumber}/transactions/{TestTransactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        result.Id.Should().Be(TestTransactionId);
    }

    [Fact]
    public async Task GetTransaction_ReturnsNotFound_ForWrongAccount()
    {
        // Arrange
        SetTestAuthToken(TestUserId);
        var transaction = _fixture.Build<Transaction>()
            .With(t => t.Id, TestTransactionId)
            .With(t => t.AccountNumber, "01999999") // Different account
            .Create();
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/v1/accounts/{TestAccountNumber}/transactions/{TestTransactionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
