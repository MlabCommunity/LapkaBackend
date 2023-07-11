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
        await _userService.Register(_context, 
            new Credentials
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
        return await _userService.LoginWeb(_context, 
            new Credentials()) 
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
        User user = await _userService.LoginMobile(_context,
            new Credentials
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
        var newToken = await _tokenService.UseToken(tokens.AccessToken!, tokens.RefreshToken!, _context);
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
        await _tokenService.RevokeToken(token.RefreshToken, _context);
        return NoContent();
    }
}
