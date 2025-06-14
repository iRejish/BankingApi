using System.ComponentModel.DataAnnotations;
using EagleBankApi.Domain.Entities;

namespace EagleBankApi.Application.Models;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public Address? Address { get; set; }

    [RegularExpression(@"^\+[1-9]\d{1,14}$")]
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
