using LapkaBackend.Domain.Enums;
using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LapkaBackend.Application.Common;
using DocumentFormat.OpenXml.InkML;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementService _managementService;
        private readonly IDataContext _dbContext;

        public ManagementController(IManagementService managementService, IDataContext dbContext)
        {
            _managementService = managementService;
            _dbContext = dbContext;
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
        ///     Lista użytkowników o wskazanej roli. Niedozwolony wybór ról: SuperAdmin, Undefined, User
        /// </summary>
        [HttpGet("/Management")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(typeof(GetUsersByRoleQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Management(Roles role)
        {

            return Ok(await _managementService.ListOfUsersWithTheSpecifiedRole(role));
        }
        
        /// <summary>
        ///     Dodanie nowych pracowników do przestrzeni shroniska przez administratora
        /// </summary>
        [HttpPost("addWorker/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddWorkerByAdmin(string userId)
        {
            await _managementService.AddWorkerByAdmin(userId);
            return NoContent();
        }

        /// <summary>
        ///      sunięcie workera z przestrzeni shroniska przez administratora
        /// </summary>
        [HttpPost("RemoveWorker/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveWorkerByAdmin(string userId)
        {
            await _managementService.RemoveWorkerByAdmin(userId);
            return NoContent();
        }

        /// <summary>
        ///      sunięcie workera z przestrzeni shroniska przez Shelter
        /// </summary>
        [HttpPost("RemoveWorkerByShelter/{userId}")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveWorkerByShelter(string email)
        {
            await _managementService.RemoveWorkerByShelter(email, await GetShelterIdByLoggedUser());
            return NoContent();
        }

        /// <summary>
        ///     Wyświetlenie pracowników shroniska
        /// </summary>
        [HttpGet("GetWorkersInShelter")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(GetUsersByRoleQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetWorkers()
        {
            return Ok(await _managementService.ListOfWorkersInShelter(await GetShelterIdByLoggedUser()));
        }




        private async Task<Guid> GetShelterIdByLoggedUser()
        {
            Guid? userId = new Guid(HttpContext.User.FindFirstValue("userId")!);
            if (userId is null)
            {
                throw new BadRequestException("invalid_user", "User doesn't exists");
            }

            var shelterId = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.ShelterId)
                .FirstOrDefaultAsync();
            if (shelterId is null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }
            return (Guid)shelterId;
        }

    }
}
