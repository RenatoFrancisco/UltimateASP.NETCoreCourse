using HotelListing.Api.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Contracts;

public interface IAuthManager
{
    Task<bool> Login(LoginDto loginDto);
    Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
}
