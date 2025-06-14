using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Data.Entities;

public class Transaction
{
    [Key]
    [RegularExpression(@"^tan-[A-Za-z0-9]+$")]
    public string Id { get; set; } = GenerateId();

    private static string GenerateId()
    {
        return $"tan-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    [Required]
    [Range(0.01, 10000.00)]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression(@"^GBP$")]
    public string Currency { get; set; } = "GBP";

    [Required]
    [RegularExpression(@"^(deposit|withdrawal)$")]
    public string Type { get; set; }

    public string? Reference { get; set; }

    [Required]
    [RegularExpression(@"^usr-[A-Za-z0-9]+$")]
    public string UserId { get; set; }

    [Required]
    [RegularExpression(@"^01\d{6}$")]
    public string AccountNumber { get; set; }

    [Required]
    public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;
}
