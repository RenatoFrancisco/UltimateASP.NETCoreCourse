using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.Users;

public class ApiUserDto : LoginDto
{
    [Required]
    public string? FisrtName { get; set; }

    [Required]
    public string? LastName { get; set; }
}
