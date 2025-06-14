namespace EagleBank.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(string userId);
}
