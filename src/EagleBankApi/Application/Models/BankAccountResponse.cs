using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EagleBankApi.Domain.Entities;

namespace EagleBankApi.Application.Models;

public class BankAccountResponse
{
    [RegularExpression(@"^01\d{6}$")] public string AccountNumber { get; set; }

    [RegularExpression("10-10-10")] public string SortCode { get; set; }

    public string Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; set; }

    [Range(0.00, 10000.00)] public decimal Balance { get; set; }

    [RegularExpression("GBP")] public string Currency { get; set; }

    public DateTime CreatedTimestamp { get; set; }
    public DateTime UpdatedTimestamp { get; set; }
}
