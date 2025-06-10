using EagleBankApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests
{
    public class ControllerTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _client;
        protected readonly EagleBankDbContext _dbContext;
        protected readonly CustomWebApplicationFactory _factory;

        protected ControllerTestBase(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Create a new scope for each test
            _dbContext = factory.Services.CreateScope().ServiceProvider.GetRequiredService<EagleBankDbContext>();

            // Ensure clean database for each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }
    }
}
