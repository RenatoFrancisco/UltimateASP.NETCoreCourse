using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.Country;

public class CreateCountryDto
{
    [Required]
    public required string Name { get; set; }
    public string? ShortName { get; set; }
}
