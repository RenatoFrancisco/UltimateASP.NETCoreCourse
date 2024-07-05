using HotelListing.Api.Core.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Core.Contracts;

public interface IAuthManager
{
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto);
    Task<string> CreateRefreshToken();
    Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
}
