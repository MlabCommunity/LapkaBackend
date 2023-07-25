using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShelterController : Controller
    {
        private readonly ISender _mediator;
        
        public ShelterController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Pobiera dane schroniska
        /// </summary>
        [HttpGet("/shelters/details/{shelterId}")]
        //[Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelter([FromRoute] Guid shelterId)
        {
            var query = new GetShelterQuery(shelterId);
            return Ok(await _mediator.Send(query)); 
        }
    }
}
