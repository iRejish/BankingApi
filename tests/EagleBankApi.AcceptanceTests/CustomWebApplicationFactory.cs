using EagleBankApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBankApi.AcceptanceTests;

public class CustomWebApplicationFactory : WebApplicationFactory<IApiMarker>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(a => a.ServiceType == typeof(DbContextOptions<EagleBankDbContext>)));

            // Add DbContext using SQLite in-memory
            services.AddDbContext<EagleBankDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Override JWT configuration for tests
            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = TestJwtTokenGenerator.TestSecretKey;
                options.Issuer = TestJwtTokenGenerator.TestIssuer;
                options.Audience = TestJwtTokenGenerator.TestAudience;
                options.ExpiryInMinutes = 60;
            });
        });

        builder.UseEnvironment("Development");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
