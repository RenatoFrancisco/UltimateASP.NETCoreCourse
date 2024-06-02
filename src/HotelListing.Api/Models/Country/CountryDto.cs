using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.Country;

public class CountryDto
{
    [Required]
    public required string Name { get; set; }
    public string? ShortName { get; set; }
}
