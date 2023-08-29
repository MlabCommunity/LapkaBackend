using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries;
public record GetShelterAdvertisementDetailsQuery(Guid PetId, double Longitude, double Latitude, Guid UserId) : IRequest<ShelterPetAdvertisementDetailsDto>;

public class GetShelterAdvertisementDetailsQueryHandler : IRequestHandler<GetShelterAdvertisementDetailsQuery, ShelterPetAdvertisementDetailsDto>
    {
        private readonly IDataContext _dbContext;

        public GetShelterAdvertisementDetailsQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetAdvertisementDetailsDto> Handle(GetShelterAdvertisementDetailsQuery query, CancellationToken cancellationToken)
        {
               var pet = _dbContext.Animals.FirstOrDefault(x => x.Id == query.PetId && x.IsVisible && !x.IsArchival);
               
               if (pet == null)
               {
                   throw new NotFoundException("invalid_id", "Advertisement not found");
               }
               
               var userShelter = await _dbContext.Users.FirstOrDefaultAsync(x => x.ShelterId == pet.Shelter.Id 
                   && x.RoleId == (int)Roles.Shelter, cancellationToken: cancellationToken);
               
               return new ShelterPetAdvertisementDetailsDto
               {
                   OrganizationName = pet.Shelter.OrganizationName,
                   OrganizationProfilePicture = userShelter!.ProfilePicture,
                   Email = userShelter.Email,
                   PetId = pet.Id,
                   Name = pet.Name,
                   Age = pet.Months,
                   IsLiked = _dbContext.Reactions.Any(reaction => reaction.AnimalId == pet.Id && reaction.UserId == query.UserId),
                   Gender = (Genders)Enum.Parse(typeof(Genders), pet.Gender),
                   Breed = pet.Species,
                   Color = pet.Marking,
                   //TODO when fixes are gonna be done, update this
                   ProfilePicture = "",
                   Distance = DistanceCalculator.CalculateDistance(
                       pet.Shelter.Latitude, pet.Shelter.Longitude, 
                       query.Latitude, query.Longitude),
                   City = pet.Shelter.City,
                   IsSterilized = pet.IsSterilized,
                   Weight = pet.Weight,
                   Photos = _dbContext.Blobs.Where(x => x.ParentEntityId == pet.Id).Select(x => x.Id).ToList(),
                   Description = pet.Description,
                   Street = pet.Shelter.Street
               };
        }
    }