using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        { 
            _authService = authService;
        }

        /// <summary>
        /// Get Auth where should be UserDTO and create new User
        /// </summary>
        /// <param name="auth"></param>
        /// <returns>Return a Created User</returns>
        #region userRegister
        [HttpPost("userRegister")]
        public async Task<ActionResult<User>> UserRegister(Auth auth)
        {
            var result = _authService.RegisterUser(auth);

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
        /// <returns>string which is JWT Token</returns>
        #region userLogin
        [HttpPost("userLogin")]
        public ActionResult<(string, string)> UserLogin(User user)
        {
            var result = _authService.LoginUser(user);

            return result;

        }
        #endregion


    }
}
