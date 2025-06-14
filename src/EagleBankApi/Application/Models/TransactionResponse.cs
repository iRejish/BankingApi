using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Application.Models;

public class TransactionResponse
{
    public string Id { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedTimestamp { get; set; }

    public string Reference { get; set; }

    [RegularExpression(@"^usr-[A-Za-z0-9]+$")]
    public string UserId { get; set; }
}
