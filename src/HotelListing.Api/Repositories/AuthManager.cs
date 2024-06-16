using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Api.Repositories
{
    public class AuthManager(IMapper mapper, 
                             UserManager<ApiUser> userManager, 
                             IConfiguration configuration) : IAuthManager
    {
        public async Task<AuthResponseDto?> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return null;

            var isValidUser = await userManager.CheckPasswordAsync(user, loginDto.Passworrd);
            if (!isValidUser) return null;

            var token = await GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Token = token
            };
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            var user = mapper.Map<ApiUser>(userDto);
            user.UserName = userDto.Email;

            var result = await userManager.CreateAsync(user, userDto.Passworrd);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "User");

            return result.Errors;
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await userManager.GetRolesAsync(user);
            var rolesClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            var userClaims =  await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("uid", user.Id),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var token = new JwtSecurityToken(issuer: configuration["JwtSettings:Issuer"], 
                                             audience: configuration["JwtSettings:Audience"],
                                             claims: claims,
                                             expires: DateTime.Now.AddMinutes(int.Parse(configuration["JwtSettings:DurationInMinutes"])),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
