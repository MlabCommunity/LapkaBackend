using Azure.Core;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

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
        [Authorize (Roles = "User")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserById(id);

            return Ok(result);
        }

           
        
        /// <summary>
        ///     Aktualizuj informacje o zalogowanym użytkowniku
        /// </summary>
        [HttpPatch]
        [Authorize (Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(User user, Guid id)
        {
            //TODO: Do przerobienia o zalogowanego użytkownika
            var result = await _userService.UpdateUser(user, id);

            return Ok(result);
        }

        /// <summary>
        ///     Usuń zalogowanego użytkownika
        /// </summary>
        [HttpDelete]
        [Authorize (Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser()
        {
            //await _userService.DeleteUser(HttpContext.User.FindFirstValue("userId")!);

            return NoContent();
        }

        /// <summary>
        ///     Aktualizuj hasło zalogowanego użytkownika.
        /// </summary>
        [HttpPatch("NewPassword")]
        [Authorize (Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> NewPassword(UserPasswordRequest request)
        {
            //await _userService.SetNewPassword(HttpContext.User.FindFirstValue("userId")!, request);

            return NoContent();
        }

        /// <summary>
        ///     Aktualizuj email zalogowanego użytkownika.
        /// </summary>
        [HttpPatch("Email")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> NewEmail(UpdateUserEmailRequest request)
        {
            //await _userService.SetNewEmail(HttpContext.User.FindFirstValue("userId")!, request);

            return NoContent();
        }

        /// <summary>
        ///     Informacje o zalogowanym użytkowniku.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        /*
        public async Task<ActionResult> GetLoggedUser()
        {
            //var result = await _userService.GetLoggedUser(HttpContext.User.FindFirstValue("userId")!);

            return Ok(result);
        }   */

        /// <summary>
        ///     Informacje o zalogowanym użytkowniku.
        /// </summary>
        [HttpPut("ConfirmUpdatedEmail/{token}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ConfirmEmail(string token)
        {
            await _userService.VerifyEmail(token);

            return NoContent();
        }

    }
}
