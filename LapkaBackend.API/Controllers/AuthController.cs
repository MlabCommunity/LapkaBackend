using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService) 
        { 
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        ///     Rejestracja użytkownika
        /// </summary>
        [HttpPost ("userRegister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UserRegister(UserRegisterDto user)
        {
            await _authService.RegisterUser(user);
            return NoContent();
        }

        /// <summary>
        ///     Rejestracja schroniska wraz z danymi użytkownika
        /// </summary>
        [HttpPost ("shelterRegister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ShelterRegister(RegistrationRequest RegistrationRequest)
        {
            await _authService.RegisterUser(RegistrationRequest.UserDto);
            await _authService.RegisterShelter(RegistrationRequest.ShelterDto);
            return NoContent();
        }
        /// <summary>
        ///     Logowanie użytkownika - zwracanie tokenów
        /// </summary>
        [HttpPost ("loginMobile")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
          public async Task<ActionResult> UserLogin(UserLoginDto user)
          {
              var result = await _authService.LoginUser(user);

              return Ok(result);
          }

        /// <summary>
        ///     Odnawia access token na podstawie refresh token
        /// </summary>
        [HttpPost ("useToken")]
        [Authorize (Roles = "User")]
        [ProducesResponseType(typeof(TokensDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshAccesToken(TokensDto tokens)
        {
            //  TODO: Wyrzucić ify do Service na Exception
            if(_authService.IsTokenValid(tokens.AccessToken))
            {
                return Ok(tokens.AccessToken);
            }

            if (!_authService.IsTokenValid(tokens.RefreshToken))
            {
                return Unauthorized("Token expired.");
            }

            var user = await _userService.FindUserByRefreshToken(tokens);
            string token = _authService.CreateAccessToken(user);

            return Ok(token);
        }

        /// <summary>
        ///     Usuwa refresh token z bazy
        /// </summary>
        [HttpPost ("revokeToken")]
        [Authorize (Roles = "User")]
        [ProducesResponseType(typeof(TokensDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task RevokeToken(TokensDto tokens)
        {
            //TODO: Zamienić tokens na tylko refresh
            await _authService.RevokeToken(tokens.RefreshToken);
        }
    }
}
