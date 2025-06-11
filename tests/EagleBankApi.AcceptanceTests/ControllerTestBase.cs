using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoFixture;
using EagleBankApi.Data;
using EagleBankApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests;

public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly EagleBankDbContext _dbContext;
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly Fixture _fixture = new();
    protected readonly IJwtTokenService _tokenGenerator;

    protected ControllerTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _tokenGenerator = _factory.Services.GetRequiredService<IJwtTokenService>();

        // Create a new scope for each test
        _dbContext = factory.Services.CreateScope().ServiceProvider.GetRequiredService<EagleBankDbContext>();

        InitializeTestDatabase();
    }

    private void InitializeTestDatabase()
    {
        // Ensure clean database for each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
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
