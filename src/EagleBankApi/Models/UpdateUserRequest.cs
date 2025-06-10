using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Models;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public Address? Address { get; set; }

    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
