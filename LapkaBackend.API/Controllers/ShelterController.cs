using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.API.Controllers
{
    [ApiController]
    [Route("shelters")]
    public class ShelterController : Controller
    {
        private readonly ISender _mediator;
        private readonly IDataContext _dbContext;

        public ShelterController(ISender mediator, IDataContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }


        /// <summary>
        ///     Updates shelter
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelter(UpdateShelterRequest request)
        {
            await _mediator.Send(new UpdateShelterCommand(await GetShelterIdByLoggedUser(), request));
            
            return NoContent();
        }

        /// <summary>
        ///     Get shelter data
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(ShelterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelter()
        {
            return Ok(await _mediator.Send(new GetShelterQuery(await GetShelterIdByLoggedUser())));
        }

        /// <summary>
        ///     Get shelter data
        /// </summary>
        [HttpGet("details/{shelterId}")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelterWithId(Guid shelterId)
        {
            return Ok(await _mediator.Send(new GetShelterQuery(shelterId)));
        }

        /// <summary>
        ///     Create pet card in shelter
        /// </summary>
        [HttpPost("cards/CreatePet")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePet(CreatePetCardRequest request)
        {
            await _mediator.Send(new CreatePetCardCommand(request, await GetShelterIdByLoggedUser()));
            return NoContent();
        }

        /// <summary>
        ///     Add pet card to archive
        /// </summary>
        [HttpPost("cards/archive/{petId}")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPetToArchive(string petId)
        {
            await _mediator.Send(new AddPetToArchiveCommand(new Guid(petId)));
            return NoContent();
        }

        // Endpointy do zwracania wyświetleń zwierząt w danym shronisku, wyświetlenia będą się dodawać w endpoincie /shelters/cards/Get/{petId}
        /// <summary>
        ///     Gets shelter's stats grouped by months in current year
        /// </summary>
        [HttpGet("/shelters/cards/archive/chart/year")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByMonths()
        { 
            return Ok(await _mediator.Send(new ShelterPetsViewsGroupByMonthsQuery(await GetShelterIdByLoggedUser())));
        }

        /// <summary>
        ///     Gets shelter's stats grouped by days in current month
        /// </summary>
        [HttpGet("cards/archive/chart/month")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByDaysInMonth()
        {
            return Ok(await _mediator.Send(new ShelterPetsViewsGroupByWeeksQuery(await GetShelterIdByLoggedUser())));
        }

        /// <summary>
        ///     Gets shelter's stats grouped by days in current week
        /// </summary>
        [HttpGet("cards/archive/chart/week")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByDaysInWeek()
        {
            return Ok(await _mediator.Send(new ShelterPetsViewsGroupByDaysInWeekQuery(await GetShelterIdByLoggedUser())));
        }

        /// <summary>
        ///     Update shelter's card
        /// </summary>
        [HttpPut("cards")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePet(UpdateShelterPetRequest request )
        {
            await _mediator.Send(new UpdatePetCommand(request));
            return NoContent();
        }

        /// <summary>
        ///     Gets shelter's pets
        /// </summary>
        [HttpGet("/shelters/cards")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(ShelterPetDetailsDtoPagedResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PetListInShelter([FromQuery]PaginationDto request, SortAnimalOptions options = SortAnimalOptions.Name)
        {
            return Ok(await _mediator.Send(new PetListInShelterQuery(request, await GetShelterIdByLoggedUser(), options)));
        }

        /// <summary>
        ///     Deletes pet from shelter
        /// </summary>
        [HttpDelete("cards/{petId}")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePet([FromRoute] string petId)
        {
            await _mediator.Send(new DeletePetCommand(new Guid(petId)));
            return NoContent();
        }


        /// <summary>
        ///     Gets shelter's pet by id
        /// </summary>
        [HttpGet("/shelters/cards/{petId}")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterPetDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPet([FromRoute] string petId)
        {
            return Ok(await _mediator.Send(
                new GetPetQuery(new Guid(petId), new Guid(HttpContext.User.FindFirstValue("userId")!))));
        }
        
        /// <summary>
        ///     Gets liked shelter's pet
        /// </summary>
        [HttpGet("/shelters/cards/liked")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterPetDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPet(
            [FromQuery] PaginationDto pagination, [FromQuery] SortLikedAnimalOption options = SortLikedAnimalOption.Name)
        {
            return Ok(await _mediator.Send(
                new GetLikedPetsQuery(pagination, await GetShelterIdByLoggedUser(), options)));
        }
        
        /// <summary>
        ///     Publikacja zwierzęcia
        /// </summary>
        [HttpPut("cards/publish/{petId}")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishPet([FromRoute] string petId)
        {
            await _mediator.Send(new PublishPetCommand(new Guid(petId)));
            return NoContent();
        }

        /// <summary>
        ///     Schowanie zwierzęcia
        /// </summary>
        [HttpPut("cards/hide/{petId}")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HidePet([FromRoute] string petId)
        {
            await _mediator.Send(new HidePetCommand(new Guid(petId)));
            return NoContent();
        }

        /// <summary>
        ///     Update danych dot. wolontariatu schroniska
        /// </summary>
        [HttpPut("/shelters/volunteering/update")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelterVolunteering(UpdateShelterVolunteeringRequest request)
        {
            var command = new UpdateShelterVolunteeringCommand(await GetShelterIdByLoggedUser(), request.BankAccountNumber, request.DonationDescription, request.DailyHelpDescription, request.TakingDogsOutDescription, request.IsDonationActive, request.IsDailyHelpActive, request.IsTakingDogsOutActive);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        ///     Pobranie danych dot. wolontariatu schroniska mobile
        /// </summary>
        [HttpGet("/shelters/volunteering/GetShelterVolunteeringMobile/{shelterId}")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterVolunteeringDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelterVolunteeringMobile([FromRoute] Guid shelterId)
        {
            var query = new GetShelterVolunteeringQuery(shelterId);
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Pobranie danych dot. wolontariatu schroniska
        /// </summary>
        [HttpGet("/shelters/volunteering/GetShelterVolunteering")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(ShelterVolunteeringDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelterVolunteering()
        {
            var query = new GetShelterVolunteeringQuery(await GetShelterIdByLoggedUser());
            return Ok(await _mediator.Send(query));
        }


        /// <summary>
        ///     Zwrócenie listy schronisk na podstawie długości i szerokości geograficznej
        /// </summary>
        [HttpGet("/shelters/volunteers/getShelterByPosition")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterByPositionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShelterByPosition([FromQuery] GetShelterByPositionQuery getShelterByPositionQuery)
        {
            return Ok(await _mediator.Send(getShelterByPositionQuery));
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
