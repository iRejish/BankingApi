using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EagleBankApi.Data.Entities;

namespace EagleBankApi.Models;

public class CreateBankAccountRequest
{
    [Required] public string Name { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; set; }
}
