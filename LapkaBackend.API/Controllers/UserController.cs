using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
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
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();

            return Ok(result);
        }

        /// <summary>
        /// Informacje na temat użytkownika o podanym Id
        /// </summary>
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

        /// <summary>
        /// Dodaje użytkownika do bazy
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            var result = await _userService.AddUser(user);

            return Ok(result);
        }

        /// <summary>
        /// Pozwala zmodyfikować użytkownikowi imie, nazwisko oraz email
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<List<User>>> UpdateUser(User user, Guid id)
        {
            var result = await _userService.UpdateUser(user, id);

            return Ok(result);
        }

        /// <summary>
        /// Usuwa użytkownika z bazy
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult<List<User>>> DeleteTeam(Guid id)
        {
            var result = await _userService.DeleteUser(id);

            return Ok(result);
        }

        /// <summary>
        /// Wyszukaj użytkownika po refreshTokenie
        /// </summary>
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
    }
}
