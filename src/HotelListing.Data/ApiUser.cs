using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Data;

public class ApiUser : IdentityUser
{
    public string? FirtName { get; set; }
    public string? LastName { get; set; }
}
