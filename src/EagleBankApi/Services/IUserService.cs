using EagleBankApi.Models;

namespace EagleBankApi.Services;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> GetUserByIdAsync(string userId);
}
