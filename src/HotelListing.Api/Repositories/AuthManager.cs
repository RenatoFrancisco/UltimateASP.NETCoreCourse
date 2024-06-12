using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Repositories
{
    public class AuthManager(IMapper mapper, UserManager<ApiUser> userManager) : IAuthManager
    {
        
        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            var user = mapper.Map<ApiUser>(userDto);
            user.UserName = userDto.Email;

            var result = await userManager.CreateAsync(user, userDto.Passworrd);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "User");

            return result.Errors;
        }
    }
}
