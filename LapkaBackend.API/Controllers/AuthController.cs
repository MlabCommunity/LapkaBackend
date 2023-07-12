using DocumentFormat.OpenXml.Bibliography;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("api/[controller]")]
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
        /// Rejestracja użytkownika
        /// </summary>
        [HttpPost("userRegister")]
        public async Task<ActionResult<User>> UserRegister(UserRegisterDto user)
        {

            var token = _authService.GenerateRefreshToken();
            var result = await _authService.RegisterUser(user);


            if(result == null)
            {
                return BadRequest("Niespójne hasła");
            }

            return Ok(result);
        }

        /// <summary>
        /// Rejestracja schroniska wraz z danymi użytkownika
        /// </summary>
        /// 
        [HttpPost("shelterRegister")]
        public async Task<ActionResult<Shelter>> ShelterRegister(RegistrationRequest RegistrationRequest)
        {
            var userResult = await _authService.RegisterUser(RegistrationRequest.UserDto);
            var shelterResult = await _authService.RegisterShelter(RegistrationRequest.ShelterDto);

            if (userResult is null || shelterResult is null)
            {
                return BadRequest("fields filled in incorrectly");
            }

            return Ok();
        }

        /// <summary>
        ///  Logowanie użytkownika
        /// </summary>
        /// <param name="user"></param>
        /// <returns>zwraca AccesToken oraz RefreshToken</returns>
        [HttpPost("userLogin")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        /*
        public async Task<ActionResult> UserLogin(UserLoginDto user)
        {
            var result = _authService.LoginUser(user);

            SetTokenInCookies(result.RefreshToken);

            return Ok(result);
        } */

        /// <summary>
        /// Odświeżanie AccesTokenu na podstawe RefreshTokenu
        /// </summary>
        [HttpPost("refreshToken")]
        public async Task<ActionResult<string>> RefreshAccesToken(TokensDto tokens)
        {
            var refreshTokenCookies = Request.Cookies["refreshToken"];

            if(_authService.IsAccesTokenValid(tokens.AccessToken))
            {
                return Ok(tokens.AccessToken);
            }

            if (!tokens.RefreshToken.Equals(refreshTokenCookies))
            {
                return Unauthorized("Invalid Refresh Token.");
            }

            if (!_authService.IsAccesTokenValid(tokens.RefreshToken))
            {
                return Unauthorized("Token expired.");
            }

            var user = await _userService.FindUserByRefreshToken(tokens);
            string token = _authService.CreateToken(user);

            return Ok(token);
        }

        /// <summary>
        /// Zapisuje Token w Cookies przeglądarki
        /// </summary>
        private void SetTokenInCookies(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        /// <summary>
        /// Usuwa refresh token z bazy
        /// </summary>
        [HttpPost("revokeToken")]
        public async Task RevokeToken(TokensDto tokens)
        {
            await _authService.RevokeToken(tokens.RefreshToken);
        }
    }
}
