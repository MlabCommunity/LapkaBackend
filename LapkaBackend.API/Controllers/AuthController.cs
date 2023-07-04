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

        #region userLogin
        [HttpPost("userLogin")]
        public ActionResult<string> UserLogin(User user)
        {
            var result = _authService.LoginUser(user);

            return result;
        }

        #endregion
    }
}
