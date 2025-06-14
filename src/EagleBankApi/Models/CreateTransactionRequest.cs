using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Models;

public class CreateTransactionRequest
{
    [Required]
    public string Type { get; set; } // "deposit" or "withdrawal"
    
    [Required]
    [Range(0.01, 10000)]
    public decimal Amount { get; set; }
    
    [Required]
    public string Currency { get; set; }
    
    public string Reference { get; set; }
}
