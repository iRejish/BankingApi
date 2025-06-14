using System.ComponentModel.DataAnnotations;

namespace EagleBank.Application.Models;

public class TransactionResponse
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Type { get; set; }
    public string Reference { get; set; }
    public string UserId { get; set; }
    public string AccountNumber { get; set; }
    public DateTime CreatedTimestamp { get; set; }
}

public class CreateTransactionRequest
{
    [Required] public string Type { get; set; } // "deposit" or "withdrawal"

    [Required] [Range(0.01, 10000)] public decimal Amount { get; set; }

    [Required] public string Currency { get; set; }

    public string Reference { get; set; }
}

public class ListTransactionsResponse
{
    public IEnumerable<TransactionResponse> Transactions { get; set; }
}
