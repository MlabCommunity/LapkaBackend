using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // Endpointy do zwracania wyświetleń zwierząt w danym shronisku, wyświetlenia będą się dodawać w endpoincie /shelters/cards/{petId}
        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według miesięcy w roku
        /// </summary>
        [HttpGet("/shelters/cards/chart/year/{shelterId}")]//Przetestować
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByMonths([FromRoute] string shelterId)
        {
            var query = new ShelterPetsViewsGroupByMonthsQuery(shelterId);
            
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według dni tygodnia w miesiącu
        /// </summary>
        [HttpGet("/shelters/cards/chart/month/{shelterId}")]//Przetestować
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupBydaysInMonth([FromRoute] string shelterId)
        {
            var query = new ShelterPetsViewsGroupByWeeksQuery(shelterId);

            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według dni tygodnia w w tygodniu
        /// </summary>
        [HttpGet("/shelters/cards/chart/week/{shelterId}")]//Przetestować
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByDaysInWeek([FromRoute] string shelterId)
        {
            var query = new ShelterPetsViewsGroupByDaysInWeekQuery(shelterId);

            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Update karty zwierzęcia
        /// </summary>
        [HttpPut("/shelters/cards/Update")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePet(UpdatePetCommand petToUpdateRequest)
        {
            await _mediator.Send(petToUpdateRequest);
            return NoContent();
        }

        /// <summary>
        ///     Wyświetlenie kart zwierząt danego schroniska z podziałem na strony
        /// </summary>
        [HttpGet("/shelters/cards/PetList")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PetList([FromQuery] PetListQuery petListQuery)
        {
            return Ok(await _mediator.Send(petListQuery));
        }

        /// <summary>
        ///     Usunięcie karty zwierzęcia
        /// </summary>
        [HttpDelete("/shelters/cards/delete/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePet([FromRoute] string petId)
        {
            var query = new DeletePetCommand(petId);
            await _mediator.Send(query);
            return NoContent();
        }

        
        /// <summary>
        ///     Wyświetlenie zwierzęcia
        /// </summary>
        [HttpGet("/shelters/cards/Get/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPet([FromRoute] string petId)
        {
            var query = new GetPetQuery(petId);
            
            return Ok(await _mediator.Send(query));
        }

        ///// <summary>
        /////     Wyświetlenie polubionych zwierząt danego schroniska z podziałem na strony
        ///// </summary>
        //[HttpPut("/shelters/cards/publish/{petId}")]
        ////[Authorize(Roles = "Shelter")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> PublishPet([FromRoute] string petId)
        //{
        //    var command = new PublishPetCommand(petId);
        //    await _mediator.Send(command);
        //    return NoContent();
        //}

        /// <summary>
        ///     Publikacja zwierzęcia
        /// </summary>
        [HttpPut("/shelters/cards/publish/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishPet([FromRoute] string petId)
        {
            var command = new PublishPetCommand(petId);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        ///     Schowanie zwierzęcia
        /// </summary>
        [HttpPut("/shelters/cards/hide/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HidePet([FromRoute] string petId)
        {
            var command = new HidePetCommand(petId);
            await _mediator.Send(command);
            return NoContent();
        }

        /*
        /// <summary>
        ///     Zwrócenie listy schronisk na podstawie długości i szerokości geograficznej
        /// </summary>
        [HttpGet("/shelters/volunteers/{longitude}{latitude}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShelterByPosition([FromRoute] string longitude, [FromRoute] string latitude)
        {
            var query = new GetShelterByPositionQuery(longitude, latitude);

            return Ok(await _mediator.Send(query));
        }   */

    }
}
