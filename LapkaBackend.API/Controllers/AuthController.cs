using LapkaBackend.API.Requests;
using LapkaBackend.Application;
using LapkaBackend.Application.IServices;
using LapkaBackend.Infrastructure.Database.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;
[ApiController]
public class AuthController : ControllerBase
{
    private IUserService _userService;
    private IDataContext _context;

    public AuthController(IUserService userService, IDataContext context)
    {
        _userService = userService;
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
    public ActionResult LoginWeb()
    {
        return _userService.LoginWeb(_context, new List<string>()) ? Ok() : StatusCode(403);
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
    public ActionResult LoginMobile([FromBody] LoginRequest user)
    {
        return _userService.LoginMobile(_context, new List<string> {user.Email, user.Password})
            ? Ok(new LoginResultDto
            {
                accessToken = _userService.GenerateToken(DateTime.UtcNow.AddMinutes(5), "access_token"),
                refreshToken = _userService.GenerateToken(DateTime.UtcNow.AddDays(3), "refresh_token")
            })
            : StatusCode(403);
    }

    /// <summary>
    ///     Rejestracja użytkownika
    /// </summary>
    [HttpPost]
    [Route("/[controller]/userRegister")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult Register([FromBody] UserRegistrationRequest newUser)
    {
        return _userService.Register(_context,
            new List<string>
                { newUser.Email, newUser.Password, newUser.FirstName, newUser.LastName, newUser.ConfirmPassword })
            ? Ok()
            : StatusCode(403);
    }
    
    /// <summary>
    /// Odnawia access token na podstawie refresh token (Working on it)
    /// </summary>
    [HttpPost]
    [Authorize] //TODO change status code to 400
    [Route("/[controller]/useToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Assuming we store user id in access token we can get refresh token internally on server without showing it on front
        // From this point we can validate refresh token and generate new access token
        // I think is somewhat secure
    public ActionResult UseToken([FromBody] LoginResultDto tokens)
    {
            //Validate access token to check if user is logged in
            //If not return 400. 
            //If he is logged in check if refresh token is valid
            //If not user needs to login again (session expired)
            //If yes generate new access token and return it
        if(_userService.ValidateToken(tokens.refreshToken!))
        {
            //TODO Generate new access token
            var newAccessToken = _userService.GenerateToken(DateTime.UtcNow.AddMinutes(5), "access_token");
            return Ok(new UseRefreshTokenResultDto()
            {
                accessToken = newAccessToken
            });
        }
        return BadRequest();

    }
    

}
