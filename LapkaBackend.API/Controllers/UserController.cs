using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        ///     Informacje o zalogowanym użytkowniku.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(GetCurrentUserDataQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetLoggedUser()
        {
            var result = await _userService.GetLoggedUser(new Guid(HttpContext.User.FindFirstValue("userId")!));

            return Ok(result);
        }

        /// <summary>
        ///     Aktualizuj informacje o zalogowanym użytkowniku
        /// </summary>
        [HttpPatch]
        [Authorize (Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser([FromBody]UpdateUserDataRequest request)
        {
            await _userService.UpdateUser(request, new Guid(HttpContext.User.FindFirstValue("userId")!));

            return NoContent();
        }

        /// <summary>
        ///     Usuń zalogowanego użytkownika
        /// </summary>
        [HttpDelete]
        [Authorize (Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser()
        {
            await _userService.DeleteUser(new Guid(HttpContext.User.FindFirstValue("userId")!));

            return NoContent();
        }

        /// <summary>
        ///     Aktualizuj hasło zalogowanego użytkownika.
        /// </summary>
         /// <response code="403">Available only for user with Łapka login provider.</response>
        [HttpPatch("NewPassword")]
        [Authorize (Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> NewPassword(UserPasswordRequest request)
        {
            await _userService.SetNewPassword(new Guid(HttpContext.User.FindFirstValue("userId")!), request);

            return NoContent();
        }

        /// <summary>
        ///     Aktualizuj email zalogowanego użytkownika.
        /// </summary>
        /// <response code="403">Available only for user with Łapka login provider.</response>
        [HttpPatch("EmailAddress")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> NewEmail(UpdateUserEmailRequest request)
        {
            await _userService.SetNewEmail(new Guid(HttpContext.User.FindFirstValue("userId")!), request);

            return NoContent();
        }

        /// <summary>
        ///     Informacje o użytkowniku o podanym id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [ProducesResponseType(typeof(GetUserDataByIdQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserById(id);

            return Ok(result);
        }

        /// <summary>
        ///     Potwierdz edycje emaila.
        /// </summary>
        [HttpPut("ConfirmUpdatedEmail/{token}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ConfirmEmail(string token)
        {
            await _userService.VerifyEmail(token);

            return NoContent();
        }
        
        /// <summary>
        ///     Usuń zdjęcie profilowe zalogowanego użytkownika.
        /// </summary>
        /// <response code="403">Available only for user with Łapka login provider.</response>
        [HttpDelete("picture")]
        [Authorize (Roles = "User,Worker,Shelter,Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProfilePicture()
        {
            await _userService.DeleteProfilePicture(new Guid(HttpContext.User.FindFirstValue("userId")!));
            
            return NoContent();
        }

    }
}
