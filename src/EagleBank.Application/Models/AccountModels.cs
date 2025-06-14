using System.ComponentModel.DataAnnotations;
using EagleBank.Domain.Entities;

namespace EagleBank.Application.Models;

public class AccountResponse
{
    [RegularExpression(@"^01\d{6}$")]
    public string AccountNumber { get; set; }

    [RegularExpression("10-10-10")]
    public string SortCode { get; set; }

    public string Name { get; set; }

    public AccountType AccountType { get; set; }

    [Range(0.00, 10000.00)]
    public decimal Balance { get; set; }

    [RegularExpression("GBP")]
    public string Currency { get; set; }

    public DateTime CreatedTimestamp { get; set; }
    public DateTime UpdatedTimestamp { get; set; }
    public string UserId { get; set; }
}

public class CreateAccountRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public AccountType AccountType { get; set; }
}

public class UpdateAccountRequest
{
    public string Name { get; set; }

    [Required]
    public AccountType AccountType { get; set; }
}

public class ListAccountsResponse
{
    public IEnumerable<AccountResponse> Accounts { get; set; }
}
