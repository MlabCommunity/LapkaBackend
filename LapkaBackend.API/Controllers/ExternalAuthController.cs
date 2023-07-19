using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class ExternalAuth : Controller
{
    private readonly IExternalAuthService _externalAuthService;

    public ExternalAuth(IExternalAuthService externalAuthService)
    {
        _externalAuthService = externalAuthService;
    }
    /// <summary>
    ///     Obsługa rejestracji/logowania usera (shelter nie może) przez google.
    /// </summary>
    /// <response code="403">Available only for user with Google login provider.</response>
    [HttpPost("google")]
    //[Authorize (Roles = "User")]
    [ProducesResponseType(typeof(LoginResultWithRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GoogleLogin(string? tokenId)
    {
        await _externalAuthService.LoginUserByGoogle(tokenId);
        return Ok();
    }
    
    /// <summary>
    ///     Obsługa rejestracji/logowania usera (shelter nie może) przez facebooka.
    /// </summary>
    /// <response code="403">Available only for user with Facebook login provider.</response>
    [HttpPost("facebook")]
    //[Authorize (Roles = "User")]
    [ProducesResponseType(typeof(LoginResultWithRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> FacebookLogin(string? userFbId, string? fbAccessToken)
    {
        return NotFound();
    }
    
    /// <summary>
    ///     Obsługa rejestracji/logowania usera (shelter nie może) przez apple.
    /// </summary>
    /// <response code="403">Available only for user with Apple login provider.</response>
    [HttpPost("apple")]
    //[Authorize (Roles = "User")]
    [ProducesResponseType(typeof(LoginResultWithRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AppleLogin(string appleAccessToken, string firstName, string lastName)
    {
        var result = await _externalAuthService.LoginUserByApple(appleAccessToken, firstName, lastName);

        return Ok(result);
    }
}