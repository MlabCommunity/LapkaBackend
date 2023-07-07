using LapkaBackend.API.Requests;
using LapkaBackend.API.Requests.Dtos;
using LapkaBackend.Application;
using LapkaBackend.Application.Exceptions;
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
    private readonly IDataContext _context;
    
    /// <summary>
    ///   Konstruktor kontrolera
    /// </summary>
    public AuthController(IUserService userService, ITokenService tokenService, IDataContext context)
    {
        _userService = userService;
        _tokenService = tokenService;
        _context = context;
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
        try
        {
            await _userService.Register(_context,
                new Dictionary<string, string>
                {
                    { "firstName", newUser.FirstName! },
                    { "lastName", newUser.LastName! },
                    { "email", newUser.Email! },
                    { "password", newUser.Password! },
                    { "confirmPassword", newUser.ConfirmPassword! }
                });
            return NoContent();
        }
        catch (PlaceholderException e)
        {
            return BadRequest(e.Message);
        }
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
        return await _userService.LoginWeb(_context, new Dictionary<string, string>()) 
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
        try
        {
            User user = await _userService.LoginMobile(_context,
                new Dictionary<string, string>
                {
                    { "email", loginRequest.Email! },
                    { "password", loginRequest.Password! }
                });

            return Ok(new LoginResultDto
            {
                accessToken = user.AccessToken,
                refreshToken = user.RefreshToken
            });
        }
        catch (PlaceholderException e)
        {
            return BadRequest(e.Message);
        }
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
        try
        {
            var newToken = await _tokenService.UseToken(tokens.accessToken!, tokens.refreshToken!, _context);
            return Ok(new UseRefreshTokenResultDto
            {
                accessToken = newToken
            });
        }
        catch (PlaceholderException e)
        {
            return BadRequest(e.Message);
        }
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
        try
        {
            await _tokenService.RevokeToken(token.refreshToken!, _context);
            return NoContent();
        }
        catch (PlaceholderException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}
