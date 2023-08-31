using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries;
public record GetAllLikedShelterAdvertisementsQuery(double Longitude, double Latitude, GetAllShelterAdvertisementsRequest Request, Guid UserId) : IRequest<ShelterPetAdvertisementDtoPagedResult>;

public class GetAllLikedShelterAdvertisementsQueryHandler : IRequestHandler<GetAllLikedShelterAdvertisementsQuery, ShelterPetAdvertisementDtoPagedResult>
    {
        private readonly IDataContext _dbContext;

        public GetAllLikedShelterAdvertisementsQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetAdvertisementDtoPagedResult> Handle(GetAllLikedShelterAdvertisementsQuery query, CancellationToken cancellationToken)
        {
            var petsAdvertisementsFromShelters = new List<ShelterPetAdvertisementDto>();
            var shelters = _dbContext.Shelters.ToList();
            
            foreach (var shelter in shelters)
            {
                var petsFromShelter = shelter.Animals.Where(x => 
                    _dbContext.Reactions.Any(reaction => reaction.AnimalId == x.Id 
                    && reaction.UserId == query.UserId)).ToList();
                
                if (petsFromShelter.Count == 0)
                {
                    continue;
                }
                
                if (query.Request.Type is not AnimalCategories.Undefined)
                {
                    petsFromShelter = petsFromShelter.Where(x => x.AnimalCategory.CategoryName == query.Request.Type.ToString()).ToList();
                }
            
                if (query.Request.Gender is not Genders.Undefined)
                {
                    petsFromShelter = petsFromShelter.Where(x => x.Gender == query.Request.Gender.ToString()).ToList();
                }
                
                petsFromShelter = petsFromShelter.Where(pet => 
                    pet.Name.Contains(query.Request.SearchText) 
                    || shelter.OrganizationName.Contains(query.Request.SearchText) 
                    || shelter.City.Contains(query.Request.SearchText)).ToList();
                
                petsFromShelter = petsFromShelter.Skip((query.Request.PageNumber - 1) * query.Request.PageSize)
                    .Take(query.Request.PageSize).ToList();
                
                var userShelter = await _dbContext.Users.FirstOrDefaultAsync(x => x.ShelterId == shelter.Id 
                    && x.RoleId == (int)Roles.Shelter, cancellationToken: cancellationToken);
                
                if (userShelter == null)
                {
                    throw new BadRequestException("invalid shelter", "Invalid shelter");
                }
                
                var petsAdvertisementsFromShelter = 
                    petsFromShelter.Select(x => new ShelterPetAdvertisementDto
                    {
                        OrganizationName = shelter.OrganizationName,
                        OrganizationProfilePicture = userShelter.ProfilePicture,
                        Email = userShelter.Email,
                        PetId = x.Id,
                        Name = x.Name,
                        Age = x.Months,
                        IsLiked = _dbContext.Reactions.Any(reaction => reaction.AnimalId == x.Id && reaction.UserId == query.UserId),
                        Gender = (Genders)Enum.Parse(typeof(Genders), x.Gender),
                        Breed = x.Species,
                        ProfilePicture = x.ProfilePhoto ?? "",
                        Distance = DistanceCalculator.CalculateDistance(
                            shelter.Latitude, shelter.Longitude, 
                            query.Latitude, query.Longitude),
                        City = shelter.City
                    }).ToList();
                petsAdvertisementsFromShelters.AddRange(petsAdvertisementsFromShelter);
            }
            
            petsAdvertisementsFromShelters = Sorter.SortAdvertisements(petsAdvertisementsFromShelters, query.Request.SortOption, 
                query.Request.AscendingSort);
            
            return new ShelterPetAdvertisementDtoPagedResult
            {
                Items = petsAdvertisementsFromShelters,
                ItemsFrom = (query.Request.PageNumber - 1) * query.Request.PageSize + 1,
                ItemsTo = query.Request.PageNumber * query.Request.PageSize,
                TotalItemsCount = petsAdvertisementsFromShelters.Count,
                TotalPages = (int)Math.Ceiling((double)petsAdvertisementsFromShelters.Count / query.Request.PageSize)
            };
        }
    }