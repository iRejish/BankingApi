using System.ComponentModel.DataAnnotations;
using EagleBank.Domain.Entities;

namespace EagleBank.Application.Models;

public class UserResponse
{
    [RegularExpression(@"^usr-[A-Za-z0-9]+$")]
    public required string Id { get; set; }

    public required string Name { get; set; }
    public required Address Address { get; set; }

    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public required string PhoneNumber { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    public DateTime CreatedTimestamp { get; set; }
    public DateTime UpdatedTimestamp { get; set; }
}

public class CreateUserRequest
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required Address Address { get; set; }

    [Required]
    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public required string PhoneNumber { get; set; }
}

public class UpdateUserRequest
{
    public string Name { get; set; }
    public Address Address { get; set; }

    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}

public class LoginUserRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

public class LoginUserResponse
{
    [Required]
    public string Token { get; set; }
    [Required]
    public UserResponse User { get; set; }
}
