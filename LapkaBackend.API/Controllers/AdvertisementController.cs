using System.Security.Claims;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[ApiController]
[Route("advertisements/shelters")]
public class AdvertisementController : Controller
{
    private readonly ISender _mediator;
    public AdvertisementController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    ///     Pobieranie listy ogłoszeń schroniska
    /// </summary>
    /// <response code="200">advertisements found or returns empty list</response>
    [HttpGet ("{longitude:double}/{latitude:double}")]
    [ProducesResponseType(typeof(ShelterPetAdvertisementDtoPagedResult), StatusCodes.Status200OK)]
    [Authorize (Roles = "User, Admin, SuperAdmin")]
    public async Task<ActionResult> GetAllShelterAdvertisements(
        double longitude, double latitude, [FromQuery] GetAllShelterAdvertisementsRequest request, [FromQuery]PaginationDto pagination)
    {
        var userId = new Guid(HttpContext.User.FindFirstValue("userId")!); 
        var items = 
            await _mediator.Send(new GetAllShelterAdvertisementsQuery(longitude, latitude, request, pagination, userId));
            
        return Ok(items);
    }
    
    /// <summary>
    ///     Pobieranie listy polubionych ogłoszeń schroniska
    /// </summary>
    /// <response code="200">advertisements found or returns empty list</response>
    [HttpGet ("like/{longitude:double}/{latitude:double}")]
    [ProducesResponseType(typeof(ShelterPetAdvertisementDtoPagedResult), StatusCodes.Status200OK)]
    [Authorize (Roles = "User, Admin, SuperAdmin")]
    public async Task<ActionResult> GetAllLikedShelterLikedAdvertisements(
        double longitude, double latitude, [FromQuery] GetAllShelterAdvertisementsRequest request, [FromQuery]PaginationDto pagination)
    {
        var userId = new Guid(HttpContext.User.FindFirstValue("userId")!); 
        var items = 
            await _mediator.Send(new GetAllLikedShelterAdvertisementsQuery(longitude, latitude, request, pagination, userId));
            
        return Ok(items);
    }
    
    /// <summary>
    ///     Pobieranie szczegółów ogłoszenia schroniska
    /// </summary>
    /// <response code="200">advertisement found</response>
    /// <response code="404">advertisement not found</response>
    [HttpGet ("{petId:Guid}/{longitude:double}/{latitude:double}")]
    [ProducesResponseType(typeof(ShelterPetAdvertisementDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize (Roles = "User, Admin, SuperAdmin")]
    public async Task<ActionResult> GetShelterAdvertisementDetails(Guid petId, double longitude, double latitude)
    {
        var userId = new Guid(HttpContext.User.FindFirstValue("userId")!);
        var item = 
            await _mediator.Send(new GetShelterAdvertisementDetailsQuery(petId, longitude, latitude, userId));
            
        return Ok(item);
    }
    
    /// <summary>
    ///     Dodanie reakcji dla zwierzaka
    /// </summary>
    /// <reposnse code="204">like added</reposnse>
    [HttpPost ("like/{petId:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize (Roles = "User, Admin, SuperAdmin")]
    public async Task<ActionResult> AddLikeToPet(Guid petId)
    {
        await _mediator.Send(new AddLikeToPetCommand(petId, new Guid(HttpContext.User.FindFirstValue("userId")!)));
            
        return NoContent();
    }
    
    /// <summary>
    ///     Usuwanie reakcji dla zwierzaka
    /// </summary>
    /// <response code="204">like removed</response>
    [HttpDelete ("like/{petId:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize (Roles = "User, Admin, SuperAdmin")]
    public async Task<ActionResult> RemoveLikeToPet(Guid petId)
    {
        await _mediator.Send(new RemoveLikeToPetCommand(petId, new Guid(HttpContext.User.FindFirstValue("userId")!)));
            
        return NoContent();
    }
}