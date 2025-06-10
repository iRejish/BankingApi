using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Models;

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
