﻿using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries;
public record GetAllShelterAdvertisementsQuery(double Longitude, double Latitude, GetAllShelterAdvertisementsRequest Request,PaginationDto Pagination, Guid UserId) : IRequest<ShelterPetAdvertisementDtoPagedResult>;

public class GetAllShelterAdvertisementQueryHandler : IRequestHandler<GetAllShelterAdvertisementsQuery, ShelterPetAdvertisementDtoPagedResult>
    {
        private readonly IDataContext _dbContext;
        public GetAllShelterAdvertisementQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetAdvertisementDtoPagedResult> Handle(GetAllShelterAdvertisementsQuery query, CancellationToken cancellationToken)
        {
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
                
                petsFromShelter = petsFromShelter.Where(pet => 
                    pet.Name.Contains(query.Pagination.SearchText)).ToList();
                
                petsFromShelter = petsFromShelter.Skip((query.Pagination.PageNumber - 1) * query.Pagination.PageSize)
                    .Take(query.Pagination.PageSize).ToList();
                
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
                query.Pagination.AscendingSort);
            
            return new ShelterPetAdvertisementDtoPagedResult
            {
                Items = petsAdvertisementsFromShelters,
                ItemsFrom = (query.Pagination.PageNumber - 1) * query.Pagination.PageSize + 1,
                ItemsTo = query.Pagination.PageNumber * query.Pagination.PageSize,
                TotalItemsCount = petsAdvertisementsFromShelters.Count,
                TotalPages = (int)Math.Ceiling((double)petsAdvertisementsFromShelters.Count / query.Pagination.PageSize)
            };
        }
    }