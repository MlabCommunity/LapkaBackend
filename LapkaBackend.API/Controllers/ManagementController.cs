using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;
using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementService _managementService;

        public ManagementController(IManagementService managementService)
        {
            _managementService = managementService;
        }

        /// <summary>
        ///     Nadanie użytkownikowi roli admina przez superadmina. Schronisko i pracownik nie mogą dostać tej roli
        /// </summary>
        [HttpPost("assignAdminRole/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AssignAdminRole([FromRoute]Guid userId)
        {
            await _managementService.AssignAdminRole(userId);
            return NoContent();
        }

        /// <summary>
        ///     Odebranie adminowi roli przez superadmina
        /// </summary>
        [HttpPost("removeAdminRole/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveAdminRole([FromRoute] Guid userId)
        {
            await _managementService.RemoveAdminRole(userId);
            return NoContent();
        }

        /// <summary>
        ///     Lista użytkowników o wskazanej roli. Niedozwolony wybór ról: SuperAdmin, Undefined, User.
        /// </summary>
        [HttpGet("/Management")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Management(Roles role)
        {

            return Ok(await _managementService.ListOfUsersWithTheSpecifiedRole(role));
        }
        /*
        /// <summary>
        ///     Dodanie nowych pracowników do przestrzeni shroniska przez administratora
        /// </summary>
        [HttpGet("addWorker/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddWorkerByAdmin(Roles role)
        {

            return Ok(await _managementService.AddWorkerByAdmin(role));
        }

        /// <summary>
        ///     Usunięcie workera przez Admina
        /// </summary>
        [HttpGet("/Management/")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveWorkerByAdmin(Roles role)
        {

            return Ok(await _managementService.AddWorkerByAdmin(role));
        }   */

    }
}
