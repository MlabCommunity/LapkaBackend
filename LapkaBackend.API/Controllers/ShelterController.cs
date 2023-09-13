using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LapkaBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        ///     update shelter
        /// </summary>
        [HttpPut("/shelters")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelter(UpdateShelterRequest request)
        {
            var command = new UpdateShelterCommand(await GetShelterIdByLoggedUser(), request.OrganizationName, request.Longitude, request.Latitude, request.City, request.Street, request.ZipCode, request.Nip, request.Krs, request.PhoneNumber);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        ///     Zwrócenie listy schronisk
        /// </summary>
        [HttpGet("/shelters")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<ShelterInListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListOfShelters()
        {
            return Ok(await _mediator.Send(new GetListOfSheltersQuery()));
        }

        /// <summary>
        ///     Pobranie danych schroniska mobile
        /// </summary>
        [HttpGet("/shelters/details/{shelterId}")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(ShelterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelterMoblile([FromRoute] Guid shelterId)
        {
            var query = new GetShelterQuery(shelterId);
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Pobranie danych schroniska
        /// </summary>
        [HttpGet("/shelters/details")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(ShelterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShelter()
        {
            var query = new GetShelterQuery(await GetShelterIdByLoggedUser());
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Utworzenie karty zwierzaka do shroniska
        /// </summary>
        [HttpPost("/shelters/cards/CreatePet")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePet(CreatePetCardRequest request)
        {
            var command = new CreatePetCardCommand(request.Name, request.Gender, request.Description, request.IsVisible, request.Months, request.IsSterilized, request.Weight, request.Marking, request.AnimalCategory, request.Species, request.Photos, await GetShelterIdByLoggedUser());
            await _mediator.Send(command);
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

        // Endpointy do zwracania wyświetleń zwierząt w danym shronisku, wyświetlenia będą się dodawać w endpoincie /shelters/cards/{petId}
        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według miesięcy w roku
        /// </summary>
        [HttpGet("/shelters/cards/chart/year")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByMonths()
        {

            var query = new ShelterPetsViewsGroupByMonthsQuery(await GetShelterIdByLoggedUser());

            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według dni tygodnia w miesiącu
        /// </summary>
        [HttpGet("/shelters/cards/chart/month")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupBydaysInMonth()
        {
            var query = new ShelterPetsViewsGroupByWeeksQuery(await GetShelterIdByLoggedUser());
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     liczba wyświetleneń kart zwierząt pogrupowana według dni obecnego tygodnia w w tygodniu
        /// </summary>
        [HttpGet("/shelters/cards/chart/week")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShelterPetsViewsGroupByDaysInWeek()
        {
            var query = new ShelterPetsViewsGroupByDaysInWeekQuery(await GetShelterIdByLoggedUser());
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
        ///     Wyświetlenie kart zwierząt z danego schroniska z paginacją mobile
        /// </summary>
        [HttpGet("/shelters/cards/petListInShelterMobile")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(PetListInShelterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PetListInShelterMoblie([FromQuery] PetListInShelterQuery petListInShelterQuery)
        {
            return Ok(await _mediator.Send(petListInShelterQuery));
        }

        /// <summary>
        ///     Wyświetlenie kart zwierząt z zalogowanego schroniska z paginacją
        /// </summary>
        [HttpGet("/shelters/cards/petListInShelter")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(PetListInShelterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PetListInShelter(
            [FromQuery] int pageNumber = 1, int pageSize = 10, string sortParam="", bool asc=false)
        {
            var query = new PetListInShelterQuery(await GetShelterIdByLoggedUser(), pageNumber, pageSize, sortParam,asc);
            return Ok(await _mediator.Send(query));
        }

        /// <summary>
        ///     Wyświetlenie kart zwierząt z wszystkich schronisk z paginacją
        /// </summary>
        [HttpGet("/shelters/cards/petList")]
        [Authorize(Roles = "Shelter")]
        [ProducesResponseType(typeof(PetListResponse), StatusCodes.Status200OK)]
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
        [HttpGet("/shelters/cards/Get/{petId}")]
        [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
        [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPet([FromRoute] Guid petId)
        {
            var query = new GetPetQuery(petId, new Guid(HttpContext.User.FindFirstValue("userId")!));
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
