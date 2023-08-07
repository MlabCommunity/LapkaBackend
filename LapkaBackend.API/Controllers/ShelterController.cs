﻿using LapkaBackend.Application.Functions.Command;
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
        [HttpPut("/shelters")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelter(UpdateShelterCommand command)
        {
            //var query = new UpdateShelterCommand(request);
            await _mediator.Send(command);

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
        ///     Utworzenie karty zwierzaka do shroniska
        /// </summary>
        [HttpPost("/shelters/cards/CreatePet")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCatCard(CreatePetCardCommand CatCard)
        {
            //var query = new CreateCatCardCommand(CatCard);
            await _mediator.Send(CatCard);
            return NoContent();
        }

        /// <summary>
        ///     Dodanie karty zwierzaka do archiwum
        /// </summary>
        [HttpPost("/shelters/cards/archive/{petId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPetToArchive([FromRoute] string petId)
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

        /// <summary>
        ///     Update danych dot. wolontariatu schroniska
        /// </summary>
        [HttpPut("/shelters/volunteering/update")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelterVolunteering( UpdateShelterVolunteeringCommand updateShelterVolunteeringCommand)
        {
            await _mediator.Send(updateShelterVolunteeringCommand);
            return NoContent();
        }

        /// <summary>
        ///     Pobranie danych dot. wolontariatu schroniska
        /// </summary>
        [HttpGet("/shelters/volunteering/get/{shelterId}")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelterVolunteering([FromRoute] string shelterId)
        {
            var query = new GetShelterVolunteeringQuery(shelterId);

            return Ok(await _mediator.Send(query));
        }

        
        /// <summary>
        ///     Zwrócenie listy schronisk na podstawie długości i szerokości geograficznej
        /// </summary>
        [HttpGet("/shelters/volunteers/getShelterByPosition")]
        //[Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShelterByPosition([FromQuery] GetShelterByPositionQuery getShelterByPositionQuery)
        {
            return Ok(await _mediator.Send(getShelterByPositionQuery));
        }   

    }
}
