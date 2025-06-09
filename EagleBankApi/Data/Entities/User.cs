using System.ComponentModel.DataAnnotations;
using EagleBankApi.Models;

namespace EagleBankApi.Data.Entities;

public class User
{
    [Key]
    [RegularExpression(@"^usr-[A-Za-z0-9]+$")]
    public string Id { get; set; } = GenerateId();

    private static string GenerateId()
    {
        return $"usr-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
    public string Name { get; set; }

    public Address Address { get; set; }

    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTimestamp { get; set; } = DateTime.UtcNow;
}
