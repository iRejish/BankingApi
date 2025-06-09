using EagleBankApi.Data.Entities;
using EagleBankApi.Models;
using EagleBankApi.Repositories;

namespace EagleBankApi.Services;

public class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
            // Validate email uniqueness
            if (await userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already in use");
            }

            var user = new User
            {
                Name = request.Name,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email.ToLower()
            };

            user = await userRepository.CreateAsync(user);
            return MapToUserResponse(user);
    }

    public async Task<UserResponse> GetUserByIdAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return MapToUserResponse(user);
    }

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CreatedTimestamp = user.CreatedTimestamp,
            UpdatedTimestamp = user.UpdatedTimestamp
        };
    }
}
