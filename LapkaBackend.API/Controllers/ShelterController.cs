﻿using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[ApiController]
[Route("shelters")]
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
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShelter(UpdateShelterCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Zwrócenie listy schronisk
    /// </summary>
    [HttpGet("")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListOfShelters()
    {
        return Ok(await _mediator.Send(new GetListOfSheltersQuery()));
    }

    /// <summary>
    ///     Pobranie danych schroniska
    /// </summary>
    [HttpGet("details/{shelterId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShelter([FromRoute] string shelterId)
    {
        var query = new GetShelterQuery(shelterId);
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    ///     Utworzenie karty zwierzaka do shroniska
    /// </summary>
    [HttpPost("cards/CreatePet")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCatCard(CreatePetCardCommand catCard)
    {
        await _mediator.Send(catCard);
        return NoContent();
    }

    /// <summary>
    ///     Dodanie karty zwierzaka do archiwum
    /// </summary>
    [HttpPost("cards/archive")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPetToArchive(AddPetToArchiveCommand petId)
    {
        await _mediator.Send(petId);
        return NoContent();
    }

    // Endpointy do zwracania wyświetleń zwierząt w danym shronisku, wyświetlenia będą się dodawać w endpoincie shelters/cards/{petId}
    /// <summary>
    ///     liczba wyświetleneń kart zwierząt pogrupowana według miesięcy w roku
    /// </summary>
    [HttpGet("cards/chart/year/{shelterId}")]
    //[Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShelterPetsViewsGroupByMonths([FromRoute] Guid shelterId)
    {
        var query = new ShelterPetsViewsGroupByMonthsQuery(shelterId);
            
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    ///     liczba wyświetleneń kart zwierząt pogrupowana według dni tygodnia w miesiącu
    /// </summary>
    [HttpGet("cards/chart/month/{shelterId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShelterPetsViewsGroupBydaysInMonth([FromRoute] Guid shelterId)
    {
        var query = new ShelterPetsViewsGroupByWeeksQuery(shelterId);
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    ///     liczba wyświetleneń kart zwierząt pogrupowana według dni obecnego tygodnia w w tygodniu
    /// </summary>
    [HttpGet("cards/chart/week/{shelterId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ShelterPetsViewsGroupByDaysInWeek([FromRoute] Guid shelterId)
    {
        var query = new ShelterPetsViewsGroupByDaysInWeekQuery(shelterId);
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    ///     Update karty zwierzęcia
    /// </summary>
    [HttpPut("cards/Update")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePet(UpdatePetCommand petToUpdateRequest)
    {
        await _mediator.Send(petToUpdateRequest);
        return NoContent();
    }

    /// <summary>
    ///     Wyświetlenie kart zwierząt z danego schroniska z paginacją
    /// </summary>
    [HttpGet("cards/petListInShelter")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PetListInShelter([FromQuery] PetListInShelterQuery petListInShelterQuery)
    {
        return Ok(await _mediator.Send(petListInShelterQuery));
    }

    /// <summary>
    ///     Wyświetlenie kart zwierząt z wszystkich schronisk z paginacją
    /// </summary>
    [HttpGet("cards/petList")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PetList([FromQuery] PetListQuery petListQuery)
    {
        return Ok(await _mediator.Send(petListQuery));
    }

    /// <summary>
    ///     Usunięcie karty zwierzęcia
    /// </summary>
    [HttpDelete("cards/delete/{petId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePet([FromRoute] Guid petId)
    {
        var query = new DeletePetCommand(petId);
        await _mediator.Send(query);
        return NoContent();
    }

        
    /// <summary>
    ///     Wyświetlenie zwierzęcia
    /// </summary>
    [HttpGet("cards/Get/{petId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPet([FromRoute] Guid petId)
    {
        var query = new GetPetQuery(petId);          
        return Ok(await _mediator.Send(query));
    }

    /// <summary>
    ///     Publikacja zwierzęcia
    /// </summary>
    [HttpPut("cards/publish/{petId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishPet([FromRoute] Guid petId)
    {
        var command = new PublishPetCommand(petId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Schowanie zwierzęcia
    /// </summary>
    [HttpPut("cards/hide/{petId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HidePet([FromRoute] Guid petId)
    {
        var command = new HidePetCommand(petId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Update danych dot. wolontariatu schroniska
    /// </summary>
    [HttpPut("volunteering/update")]
    [Authorize(Roles = "Shelter")]
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
    [HttpGet("volunteering/get/{shelterId}")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShelterVolunteering([FromRoute] Guid shelterId)
    {
        var query = new GetShelterVolunteeringQuery(shelterId);
        return Ok(await _mediator.Send(query));
    }

        
    /// <summary>
    ///     Zwrócenie listy schronisk na podstawie długości i szerokości geograficznej
    /// </summary>
    [HttpGet("volunteers/getShelterByPosition")]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShelterByPosition([FromQuery] GetShelterByPositionQuery getShelterByPositionQuery)
    {
        return Ok(await _mediator.Send(getShelterByPositionQuery));
    }   

}