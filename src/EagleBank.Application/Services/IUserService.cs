using EagleBank.Application.Models;

namespace EagleBank.Application.Services;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> GetUserByIdAsync(string userId);
    Task<LoginUserResponse> LoginUser(string email, string password);
    Task<UserResponse> UpdateUserAsync(string userId, UpdateUserRequest request);
    Task DeleteUserAsync(string userId);
}
