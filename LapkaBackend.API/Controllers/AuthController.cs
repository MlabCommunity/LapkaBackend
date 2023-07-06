using LapkaBackend.Application.Common;
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
        /// <param name="auth"></param>
        /// <returns>Return a Created User</returns>
        #region userRegister
        [HttpPost("userRegister")]
        public async Task<ActionResult<User>> UserRegister(Auth auth) // TODO: Dodać UserDTO 
        {
            var result = await _authService.RegisterUser(auth);

            if(result == null)
            {
                return BadRequest("Niespójne hasła");
            }

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Let Registered User to SginIn and get JWT Token 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>returns JWT string</returns>
        #region userLogin
        [HttpPost("userLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> UserLogin(User user)
        {
            var result = _authService.LoginUser(user);

            var newRefreshToken = _authService.GenerateRefreshToken();
            SetTokenInCookies(newRefreshToken);
            _authService.SaveRefreshToken(user, newRefreshToken); // sprawdzić czy działa

            return "acces: " + result + " refresh: " + newRefreshToken.Token;
        }
        #endregion

        #region RefreshToken
        [HttpPost("refreshToken")]
        public async Task<ActionResult<string>> RefreshToken(string refreshToken)
        {
            var refreshTokenCookies = Request.Cookies["refreshToken"];
            var user = _userService.FindUserByRefreshToken(refreshToken);

            if (!refreshToken.Equals(refreshTokenCookies))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.Result.TokenExpire < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = _authService.CreateToken(user.Result);

            return Ok(token);
        }
        #endregion


        private async Task SetTokenInCookies(RefreshToken token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = token.Expire
            };
            Response.Cookies.Append("refreshToken", token.Token, cookieOptions);
        }
    }
}
