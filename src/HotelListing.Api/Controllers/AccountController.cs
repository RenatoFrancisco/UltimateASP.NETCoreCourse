using HotelListing.Api.Core.Contracts;
using HotelListing.Api.Core.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAuthManager authManager, ILogger<AccountController> logger) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Register(ApiUserDto apiUserDto)
    {
        logger.LogInformation($"Registration attempt for user with email {apiUserDto.Email}.");

        var errors = await authManager.Register(apiUserDto);
        if (errors.Any())
        {
            logger.LogWarning($"Error during Registration attempt for user with email {apiUserDto.Email}.");

            foreach (var error in errors)
                ModelState.AddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        logger.LogInformation($"Registration for user with email {apiUserDto.Email} has been successfully completed.");

        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        logger.LogInformation($"Login attempt for user with email {loginDto.Email}.");

        var authResponse = await authManager.Login(loginDto);

        if (authResponse is null)
        {
            logger.LogWarning($"User with email {loginDto.Email} not authorized.");
            return Unauthorized();
        }

        logger.LogInformation($"Login for user with email {loginDto.Email} has been successfully authorized.");

        return Ok(authResponse);
    }

    [HttpPost("refreshtoken")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken(AuthResponseDto request)
    {
        logger.LogInformation($"Refresh token attempt for user with id {request.UserId}.");

        var authResponse = await authManager.VerifyRefreshToken(request);

        if (authResponse is null)
        {
            logger.LogWarning($"User with id {request.UserId} not authorized.");
            return Unauthorized();
        }

        logger.LogInformation($"Refresh token for user with id {request.UserId} has been successfully generated.");

        return Ok(authResponse);
    }
}
