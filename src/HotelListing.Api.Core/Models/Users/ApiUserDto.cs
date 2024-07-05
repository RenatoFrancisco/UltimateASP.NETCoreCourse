using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Core.Models.Users;

public class ApiUserDto : LoginDto
{
    [Required]
    public string? FisrtName { get; set; }

    [Required]
    public string? LastName { get; set; }
}
