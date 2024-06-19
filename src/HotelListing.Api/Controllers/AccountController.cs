using HotelListing.Api.Contracts;
using HotelListing.Api.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAuthManager authManager): ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Register(ApiUserDto apiUserDto)
    {
        var errors = await authManager.Register(apiUserDto);
        if (errors.Any())
        { 
            foreach (var error in errors)
                ModelState.AddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(LoginDto  loginDto)
    {
        var authResponse = await authManager.Login(loginDto);

        if (authResponse is null) return Unauthorized();

        return Ok(authResponse);
    }

    [HttpPost("refreshtoken")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken(AuthResponseDto request)
    {
        var authResponse = await authManager.VerifyRefreshToken(request);

        if (authResponse is null) return Unauthorized();

        return Ok(authResponse);
    }
}
