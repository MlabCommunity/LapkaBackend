using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        ///     Informacje o użytkowniku o podanym id
        /// </summary>
        [HttpGet("{id}")]
        //[Authorize (Roles = "User")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            return Ok(await _userService.GetUserById(id));
        }
        
        /// <summary>
        ///     Aktualizuj informacje o zalogowanym użytkowniku
        /// </summary>
        [HttpPatch]
        //[Authorize (Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(User user, Guid id)
        {
            var result = await _userService.UpdateUser(user, id);

            return Ok(result);
        }

        /// <summary>
        ///     Usuń zalogowanego użytkownika
        /// </summary>
        [HttpDelete]
        //[Authorize (Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTeam(Guid id)
        {
            await _userService.DeleteUser(id);

            return NoContent();
        }
    }
}
