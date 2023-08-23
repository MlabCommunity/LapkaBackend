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
public record GetAllShelterAdvertisementsQuery(GetAllShelterAdvertisementsRequest Request) : IRequest<ShelterPetAdvertisementDtoPagedResult>;

public class GetAllShelterAdvertisementQueryHandler : IRequestHandler<GetAllShelterAdvertisementsQuery, ShelterPetAdvertisementDtoPagedResult>
    {
        private readonly IDataContext _dbContext;
        public GetAllShelterAdvertisementQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetAdvertisementDtoPagedResult> Handle(GetAllShelterAdvertisementsQuery query, CancellationToken cancellationToken)
        {
            if (query.Request.PageSize <= 0)
            {
                throw new BadRequestException("invalid_page_size", "Page size must be greater than 0");
            }
            if (query.Request.PageNumber <= 0)
            {
                throw new BadRequestException("invalid_page_number", "Page number must be greater than 0");
            }
            var petsAdvertisementsFromShelters = new List<ShelterPetAdvertisementDto>();
            var shelters = _dbContext.Shelters.Include(x => x.Animals).ToList();
            foreach (var shelter in shelters)
            {
                var petsFromShelter = shelter.Animals;
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
                        IsLiked = _dbContext.Reactions.Any(reaction => reaction.AnimalId == x.Id && reaction.UserId == query.Request.UserId),
                        Gender = (Genders)Enum.Parse(typeof(Genders), x.Gender),
                        Breed = x.Species,
                        //TODO when fixes are gonna be done, update this
                        ProfilePicture = "",
                        Distance = DistanceCalculator.CalculateDistance(
                            shelter.Latitude, shelter.Longitude, 
                            query.Request.Latitude, query.Request.Longitude),
                        City = shelter.City
                    }).ToList();
                petsAdvertisementsFromShelters.AddRange(petsAdvertisementsFromShelter);
            }
            petsAdvertisementsFromShelters = query.Request.SortingType switch
            {
                SortingType.Ascending => query.Request.SortOption switch
                {
                    SortAdvertisementOptions.Name => petsAdvertisementsFromShelters
                        .OrderBy(x => x.Name)
                        .ToList(),
                    SortAdvertisementOptions.Age => petsAdvertisementsFromShelters
                        .OrderBy(x => x.Age)
                        .ToList(),
                    SortAdvertisementOptions.Distance => petsAdvertisementsFromShelters
                        .OrderBy(x => x.Distance)
                        .ToList(),
                    SortAdvertisementOptions.IsLiked => petsAdvertisementsFromShelters
                        .OrderBy(x => x.IsLiked)
                        .ToList(),
                    _ => petsAdvertisementsFromShelters
                },
                SortingType.Descending => query.Request.SortOption switch
                {
                    SortAdvertisementOptions.Name => petsAdvertisementsFromShelters
                        .OrderByDescending(x => x.Name)
                        .ToList(),
                    SortAdvertisementOptions.Age => petsAdvertisementsFromShelters
                        .OrderByDescending(x => x.Age)
                        .ToList(),
                    SortAdvertisementOptions.Distance => petsAdvertisementsFromShelters
                        .OrderByDescending(x => x.Distance)
                        .ToList(),
                    SortAdvertisementOptions.IsLiked => petsAdvertisementsFromShelters
                        .OrderByDescending(x => x.IsLiked)
                        .ToList(),
                    _ => petsAdvertisementsFromShelters
                },
                _ => petsAdvertisementsFromShelters
            };
            //return paged result
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