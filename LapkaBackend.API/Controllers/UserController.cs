using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Pobiera listę użytkowników
        /// </summary>
        #region GetAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Informacje na temat użytkownika o podanym Id
        /// </summary>
        #region GetUserById
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var result =await _userService.GetUserById(id);

            if(result==null)
            {
                return NotFound("User doesn't exists");
            }

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Dodaje użytkownika do bazy
        /// </summary>
        #region AddUser
        [HttpPost]
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            var result = await _userService.AddUser(user);

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Pozwala zmodyfikować użytkownikowi imie, nazwisko oraz email
        /// </summary>
        #region UpdateUser
        [HttpPut]
        public async Task<ActionResult<List<User>>> UpdateUser(User user, Guid id)
        {
            var result = await _userService.UpdateUser(user, id);

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Usuwa użytkownika z bazy
        /// </summary>
        #region DeleteTeam
        [HttpDelete]
        public async Task<ActionResult<List<User>>> DeleteTeam(Guid id)
        {
            var result = await _userService.DeleteUser(id);

            return Ok(result);
        }
        #endregion

        /// <summary>
        /// Wyszukaj użytkownika po refreshTokenie
        /// </summary>
        #region GetUserByRefreshToken
        [HttpGet("GetUserByRefreshToken")]
        public async Task<ActionResult<User>> GetUserByRefreshToken(TokensDto token)
        {
            var result =await _userService.FindUserByRefreshToken(token);

            if(result == null)
            {
                return NotFound("User doesn't exists");
            }

            return Ok(result);
        }
        #endregion
    }
}
