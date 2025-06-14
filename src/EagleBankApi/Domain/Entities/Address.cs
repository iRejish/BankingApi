using System.ComponentModel.DataAnnotations;

namespace EagleBankApi.Domain.Entities;

public class Address
{
    [Required]
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }

    [Required]
    public required string Town { get; set; }

    [Required]
    public required string County { get; set; }

    [Required]
    public required string Postcode { get; set; }
}
