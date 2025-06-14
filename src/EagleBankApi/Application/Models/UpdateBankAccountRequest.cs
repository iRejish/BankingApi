using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EagleBankApi.Domain.Entities;

namespace EagleBankApi.Application.Models;

public class UpdateBankAccountRequest
{
    public string Name { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; set; }
}
