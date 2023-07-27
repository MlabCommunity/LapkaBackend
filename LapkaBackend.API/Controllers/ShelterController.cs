using LapkaBackend.Application.Functions.Posts;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
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
        ///     update shelter
        /// </summary>
        [HttpPut("/shelters{shelterId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelter([FromRoute] Guid shelterId, UpdateShelterRequest request)
        {
            var query = new UpdateShelterCommand(request, shelterId);
            await _mediator.Send(query);

            return NoContent();
        }

        /// <summary>
        ///     Zwrócenie listy schronisk
        /// </summary>
        [HttpGet("/shelters")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOfShelters()
        {
            var query = new GetListOfSheltersQuery();
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Pobranie danych schroniska
        /// </summary>
        [HttpGet("/shelters/details/{shelterId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelter([FromRoute] Guid shelterId)
        {
            var query = new GetShelterQuery(shelterId);
            return Ok(await _mediator.Send(query)); 
        }

        /// <summary>
        ///     Utworzenie karty psa do shroniska
        /// </summary>
        [HttpPost("/shelters/cards/dog")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateDogCart(DogCard dogCard)
        {
            var query = new CreateDogCardCommand(dogCard);
            await _mediator.Send(query);
            return NoContent();
        }

        /// <summary>
        ///     Utworzenie karty kota do shroniska
        /// </summary>
        [HttpPost("/shelters/cards/cat")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCatCard(CatCard CatCard)
        {
            var query = new CreateCatCardCommand(CatCard);
            await _mediator.Send(query);
            return NoContent();
        }

        /// <summary>
        ///     Utworzenie karty innego zwierzęcia(Undefined) do shroniska
        /// </summary>
        [HttpPost("/shelters/cards/UndefinedAnimal")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateUndefinedAnimalCard(UndefinedAnimalCard UndefinedAnimalCard)
        {
            var query = new CreateUndefinedAnimalCardCommand(UndefinedAnimalCard);
            await _mediator.Send(query);
            return NoContent();
        }

        /// <summary>
        ///     Dodanie karty zwierzaka do archiwum
        /// </summary>
        [HttpPost("/shelters/cards/archive/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPetToArchive([FromRoute] Guid petId)
        {
            var query = new AddPetToArchiveCommand(petId);
            await _mediator.Send(query);
            return NoContent();
        }

        

    }
}
