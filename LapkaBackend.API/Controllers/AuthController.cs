using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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
        #region userRegister
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
        #endregion

        /// <summary>
        ///  Logowanie użytkownika
        /// </summary>
        /// <param name="user"></param>
        /// <returns>zwraca AccesToken oraz RefreshToken</returns>
        #region userLogin
        [HttpPost("userLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResultDto>> UserLogin(UserLoginDto user)
        {   
         
            var result = _authService.LoginUser(user);

            LoginResultDto tokens = new LoginResultDto();

            var findedUser = await _userService.FindUserByEmail(user.Email);
            tokens.RefreshToken = findedUser.RefreshToken;

            if (!_authService.IsAccesTokenValid(tokens.RefreshToken))
            { 
                var newRefreshToken = _authService.GenerateRefreshToken();
                SetTokenInCookies(newRefreshToken);
                await _authService.SaveRefreshToken(user, newRefreshToken);
                tokens.RefreshToken = newRefreshToken;
            }

            tokens.AccessToken = result;
            

            return tokens;
        }
        #endregion

        /// <summary>
        /// Odświeżanie AccesTokenu na podstawe RefreshTokenu
        /// </summary>
        #region RefreshToken
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
        #endregion

        /// <summary>
        /// Zapisuje Token w Cookies przeglądarki
        /// </summary>
        #region SetTokenInCookies
        private void SetTokenInCookies(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        #endregion

        /// <summary>
        /// Usuwa refresh token z bazy
        /// </summary>
        #region RevokeToken
        [HttpPost("revokeToken")]
        public async Task RevokeToken(TokensDto tokens)
        {
            await _authService.RevokeToken(tokens.RefreshToken);
        }
        #endregion
    }
}
