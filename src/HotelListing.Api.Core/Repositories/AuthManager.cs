using AutoMapper;
using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Api.Core.Repositories;

public class AuthManager(IMapper mapper,
                         UserManager<ApiUser> userManager,
                         IConfiguration configuration) : IAuthManager
{
    private const string LoginProvider = "HotelListingApi";
    private const string TokenName = "RefreshToken";

    private ApiUser? _user;

    public async Task<AuthResponseDto?> Login(LoginDto loginDto)
    {
        _user = await userManager.FindByEmailAsync(loginDto.Email);
        if (_user is null) return null;

        var isValidUser = await userManager.CheckPasswordAsync(_user, loginDto.Passworrd);
        if (!isValidUser) return null;

        var token = await GenerateToken();

        return new AuthResponseDto
        {
            UserId = _user.Id,
            Token = token
        };
    }

    public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
    {
        _user = mapper.Map<ApiUser>(userDto);
        _user.UserName = userDto.Email;

        var result = await userManager.CreateAsync(_user, userDto.Passworrd);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(_user, "User");

        return result.Errors;
    }

    public async Task<string> CreateRefreshToken()
    {
        await userManager.RemoveAuthenticationTokenAsync(_user, LoginProvider, TokenName);
        var newRefreshToken = await userManager.GenerateUserTokenAsync(_user, LoginProvider, TokenName);
        await userManager.SetAuthenticationTokenAsync(_user, LoginProvider, TokenName, newRefreshToken);

        return newRefreshToken;
    }

    public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var userName = tokenContent.Claims.ToList()
            .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
        _user = await userManager.FindByNameAsync(userName);

        if (_user is null || _user.Id != request.UserId) return null;

        var isValidRefreshToken = await userManager.VerifyUserTokenAsync(_user, LoginProvider, TokenName, request.RefreshToken);
        if (isValidRefreshToken)
            return new AuthResponseDto
            {
                Token = await GenerateToken(),
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken(),
            };

        await userManager.UpdateSecurityStampAsync(_user);

        return null;
    }

    private async Task<string> GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var roles = await userManager.GetRolesAsync(_user);
        var rolesClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
        var userClaims = await userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, _user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, _user.Email),
            new("uid", _user.Id),
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