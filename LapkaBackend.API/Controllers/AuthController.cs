using LapkaBackend.API.Requests;
using LapkaBackend.API.Requests.Dtos;
using LapkaBackend.Application;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Entities;
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
        User user = await _userService.LoginMobile(_context,
            new Dictionary<string, string>
            {
                {"email", loginRequest.Email }, 
                { "password", loginRequest.Password }
            });
        
        return user != null 
            ? Ok(new LoginResultDto
            {
                accessToken = user.AccessToken,
                refreshToken = user.RefreshToken
            })
            : StatusCode(400);
    }

    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpPost]
    [Route("/[controller]/userRegister")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Register([FromBody] UserRegistrationRequest newUser)
    {
        return await _userService.Register(_context,
            new Dictionary<string, string>
            {
                { "firstName", newUser.FirstName },
                { "lastName", newUser.LastName },
                { "email", newUser.Email },
                { "password", newUser.Password },
                { "confirmPassword", newUser.ConfirmPassword }
            })
            ? Ok()
            : StatusCode(400);
    }
    
    /// <summary>
    /// Odnawia access token na podstawie refresh token (Working on it)
    /// </summary>
    [HttpPost]
    [Route("/[controller]/useToken")]
    [ProducesResponseType(typeof(UseRefreshTokenResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UseToken([FromBody] LoginResultDto tokens)
    {
        var newToken = await _tokenService.UseToken(tokens.accessToken, tokens.refreshToken, _context);
        
        return Ok(new UseRefreshTokenResultDto
        {
            accessToken = newToken
        });
    }
    

}
