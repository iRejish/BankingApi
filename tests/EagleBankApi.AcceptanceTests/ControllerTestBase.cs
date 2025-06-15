using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using EagleBank.Domain.Entities;
using EagleBank.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests;

public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    protected EagleBankDbContext _dbContext;
    protected readonly CustomWebApplicationFactory _factory;
    private IServiceScope _serviceScope;

    protected readonly Fixture _fixture = new();
    protected readonly JsonSerializerOptions _jsonOptions;

    // Test data constants
    protected const string TestUserId = "usr-test123";
    protected const string TestAccountNumber = "01234567";
    protected const string TestAuthScheme = "TestScheme";

    protected ControllerTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public async Task InitializeAsync()
    {
        // Create isolated scope for DB operations
        _serviceScope = _factory.Services.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<EagleBankDbContext>();

        await InitializeTestDatabaseAsync();
    }

    protected virtual async Task InitializeTestDatabaseAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        await SeedTestUserAsync();
        await SeedTestAccountAsync();
    }

    protected virtual async Task SeedTestUserAsync()
    {
        if (!_dbContext.Users.Any(u => u.Id == TestUserId))
        {
            var user = _fixture.Build<User>()
                .With(x=>x.Id, TestUserId)
                .Create();

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }

    protected virtual async Task SeedTestAccountAsync()
    {
        if (!_dbContext.Accounts.Any(a => a.AccountNumber == TestAccountNumber))
        {
            var account = new Account
            {
                AccountNumber = TestAccountNumber,
                SortCode = "10-10-10",
                Name = "Test Account",
                AccountType = AccountType.Personal,
                Balance = 1000.00m,
                Currency = "GBP",
                UserId = TestUserId
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
        }
    }

    protected void SetTestAuthToken(string userId)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthScheme, userId);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _serviceScope?.Dispose();
        _client.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
