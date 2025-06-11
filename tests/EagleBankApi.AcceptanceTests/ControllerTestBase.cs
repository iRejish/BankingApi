using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoFixture;
using EagleBankApi.Data;
using EagleBankApi.Data.Entities;
using EagleBankApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests;

public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    protected EagleBankDbContext _dbContext;
    protected readonly CustomWebApplicationFactory _factory;
    private IServiceScope _serviceScope;

    protected readonly Fixture _fixture = new();
    protected readonly IJwtTokenService _tokenGenerator;

    // Test data constants
    protected const string TestUserId = "usr-test123";
    protected const string TestAccountNumber = "01234567";
    protected const string TestAuthScheme = "TestScheme";

    protected ControllerTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _tokenGenerator = _factory.Services.GetRequiredService<IJwtTokenService>();
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
                UserId = TestUserId, // Only set the foreign key
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow
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

    protected async Task<UserResponse> CreateTestUser()
    {
        // Arrange
        var request = _fixture.Build<CreateUserRequest>()
            .With(x => x.Email, "a@b.com")
            .With(x => x.PhoneNumber, "+123456789")
            .Create();

        // Act
        var response = await _client.PostAsJsonAsync("/v1/users", request);
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    protected void AuthenticateClient(string userId)
    {
        var token = _tokenGenerator.GenerateToken(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
