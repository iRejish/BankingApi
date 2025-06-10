using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Models;

public class CreateUserRequest
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required Address Address { get; set; }

    [Required]
    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public required string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
