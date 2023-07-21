using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        { 
            _authService = authService;
        }

        /// <summary>
        ///     Rejestracja użytkownika
        /// </summary>
        [HttpPost ("userRegister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UserRegister(UserRegistrationRequest request)
        {
            
            await _authService.RegisterUser(request);
            return NoContent();
        }

        /// <summary>
        ///     Rejestracja schroniska wraz z danymi użytkownika
        /// </summary>
        [HttpPost ("shelterRegister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ShelterRegister(ShelterWithUserRegistrationRequest request)
        {
            await _authService.RegisterShelter(request);

            return NoContent();
        }

        /// <summary>
        ///     Potwierdzenie maila podanego przy rejestracji
        /// </summary>
        [HttpPost("confirmEmail{token}")]
        [Authorize(Roles = "User, Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ConfirmEmail([FromRoute] string token)
        {
            await _authService.ConfirmEmail(token);

            return NoContent();
        }

        /// <summary>
        ///     Logowanie schroniska
        /// </summary>
        [HttpPost("loginWeb")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LoginWeb(LoginRequest request)
        {
           var result = await _authService.LoginShelter(request);

            return Ok(result);
        }
        /// <summary>
        ///     Logowanie użytkownika - zwracanie tokenów
        /// </summary>
        [HttpPost ("loginMobile")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
          public async Task<ActionResult> UserLogin(LoginRequest request)
          { 
              var result = await _authService.LoginUser(request);

              return Ok(result);
          }

        /// <summary>
        ///     Odnawia access token na podstawie refresh token
        /// </summary>
        [HttpPost ("useToken")]
        [Authorize (Roles = "User,Worker,Shelter,SuperAdmin,Admin")]
        [ProducesResponseType(typeof(UseRefreshTokenResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshAccesToken(UseRefreshTokenRequest request)
        {
            //  TODO: Wyrzucić ify do Service na Exception
            if (_authService.IsTokenValid(request.AccessToken))
            {
                return Ok(request.AccessToken);
            }

            if (!_authService.IsTokenValid(request.RefreshToken))
            {
                return Unauthorized("Token expired.");
            }

            var token = await _authService.RefreshAccessToken(request);

            return Ok(token);
        }

        /// <summary>
        ///     Usuwa refresh token z bazy
        /// </summary>
        [HttpPost ("revokeToken")]
        [Authorize(Roles = "User,Worker,Shelter,SuperAdmin,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RevokeToken([FromBody] TokenRequest request)
        {
            await _authService.RevokeToken(request);
            return NoContent();
        }

        /// <summary>
        ///     Wysłanie maila z linkiem do zmiany hasła
        /// </summary>
        [HttpPost("resetPassword")]
        [Authorize(Roles = "User,Worker,Shelter,SuperAdmin,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetPassword(string emailAddress)
        {
            await _authService.ResetPassword(emailAddress);
            return NoContent();
        }

        /// <summary>
        ///     Ustawie nowego hasła.
        /// </summary>       
        [HttpPost("setPassword/{token}")]
        [Authorize(Roles = "User,Worker,Shelter,SuperAdmin,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SetNewPassword(ResetPasswordRequest resetPasswordRequest, [FromRoute] string token)
        {
            await _authService.SetNewPassword(resetPasswordRequest, token);
            return NoContent();
        } 
    }
}
