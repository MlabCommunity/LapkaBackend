using LapkaBackend.API.Requests;
using LapkaBackend.API.Requests.Dtos;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

/// <summary>
///     Kontroler do obsługi logowania i rejestracji użytkowników
///  </summary>
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    /// <summary>
    ///   Konstruktor kontrolera
    /// </summary>
    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }
    
    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpPost]
    [Route("/[controller]/userRegister")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UserRegister([FromBody] UserRegistrationRequest newUser)
    {
        await _userService.Register(new Credentials
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Password = newUser.Password,
                ConfirmPassword = newUser.ConfirmPassword
            });
        return NoContent();
    }
    
    /// <summary>
    ///     Rejestracja schroniska wraz z danymi użytkownika
    /// </summary>
    [HttpPost]
    [Route("/[controller]/shelterRegister")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ShelterRegister([FromBody] ShelterWithUserRegistrationRequest newShelter)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Logowanie pracownika i schroniska - zwracanie tokenów (Not implemented)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost]
    [Route("/[controller]/loginWeb")]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LoginWeb()
    {
        return await _userService.LoginWeb(new Credentials()) 
            ? Ok() : StatusCode(403);
    }
    
    /// <summary>
    ///     Logowanie użytkownika - zwracanie tokenów
    /// </summary>
    [HttpPost]
    [Route("/[controller]/loginMobile")]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LoginMobile([FromBody] LoginRequest loginRequest)
    {
        var user = await _userService.LoginMobile(new Credentials
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password
            });

        return Ok(new LoginResultDto
        {
            AccessToken = user.AccessToken,
            RefreshToken = user.RefreshToken
        });
    }

    /// <summary>
    ///     Odnawia access token na podstawie refresh token
    /// </summary>
    [HttpPost]
    [Authorize (Roles = "User")]
    [Route("/[controller]/useToken")]
    [ProducesResponseType(typeof(UseRefreshTokenResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UseToken([FromBody] LoginResultDto tokens)
    {
        var newToken = await _tokenService.UseToken(tokens.AccessToken!, tokens.RefreshToken!);
        return Ok(new UseRefreshTokenResultDto
        {
            AccessToken = newToken
        });
    }
    
    /// <summary>
    ///     Usuwa refresh token z bazy
    /// </summary>
    [HttpPost]
    [Authorize (Roles = "User")]
    [Route("/[controller]/revokeToken")]
    [ProducesResponseType(typeof(UseRefreshTokenResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RevokeToken([FromBody] TokenRequest token)
    {
        await _tokenService.RevokeToken(token.RefreshToken);
        return NoContent();
    }
}
