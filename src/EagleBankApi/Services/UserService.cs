using EagleBankApi.Data.Entities;
using EagleBankApi.Models;
using EagleBankApi.Repositories;

namespace EagleBankApi.Services;

public class UserService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService) : IUserService
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
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return MapToUserResponse(user);
    }

    public async Task<LoginUserResponse> LoginUser(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        //TODO: implement password hash and verify password

        var token = jwtTokenService.GenerateToken(user.Id);
        return new LoginUserResponse(user.Id, token);
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
