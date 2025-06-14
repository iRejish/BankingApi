using System.ComponentModel.DataAnnotations;

namespace EagleBank.Domain.Entities;

public class Account
{
    [Key]
    [RegularExpression(@"^01\d{6}$")]
    public string AccountNumber { get; set; }

    public string SortCode { get; set; } = "10-10-10";
    public string Name { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "GBP";
    public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTimestamp { get; set; } = DateTime.UtcNow;

    [RegularExpression(@"^usr-[A-Za-z0-9]+$")]
    public string UserId { get; set; }

    public User User { get; set; }
}

public enum AccountType
{
    Personal,
    Savings,
    Business
}
