using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IManagementService _managementService;

        public ManagementController(IAuthService authService, IUserService userService, IManagementService managementService)
        {
            _authService = authService;
            _userService = userService;
            _managementService = managementService;
        }


        [HttpPost("assignAdminRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AssignAdminRole(Guid userId)
        {
            await _managementService.AssignRemoveAdminRole(userId, "Admin");
            return NoContent();
        }

        [HttpPost("removeAdminRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveAdminRole(Guid userId)
        {
            await _managementService.AssignRemoveAdminRole(userId, "Worker");
            return NoContent();
        }

        [HttpGet("/Management")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Management(string roleName)
        {
            
            return Ok(await _managementService.ListOfUsersWithTheSpecifiedRole(roleName));
        }
    }
}
