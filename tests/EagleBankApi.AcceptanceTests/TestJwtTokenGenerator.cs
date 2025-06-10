using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EagleBankApi.AcceptanceTests;

public static class TestJwtTokenGenerator
{
    public const string TestSecretKey = "test-secret-32-chars-long-1234567890";
    public const string TestIssuer = "TestIssuer";
    public const string TestAudience = "TestAudience";

    public static string GenerateToken(string userId, DateTime? expiration = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: new[] { new Claim("sub", userId) },
            expires: expiration ?? DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateExpiredToken(string userId)
        => GenerateToken(userId, DateTime.UtcNow.AddHours(-1));

    public static string GenerateWrongAudienceToken(string userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: "wrong-audience",
            claims: new[] { new Claim("sub", userId) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
