using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Domain.Enums;


namespace LapkaBackend.Application.Functions.Queries
{
    public record PetListInShelterQuery(PaginationDto Pagination, Guid ShelterId, SortAnimalOptions SortOption) : IRequest<ShelterPetDetailsDtoPagedResult>;

    public class PetListInShelterQueryHandler : IRequestHandler<PetListInShelterQuery, ShelterPetDetailsDtoPagedResult>
    {
        private readonly IDataContext _dbContext;

        public PetListInShelterQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetDetailsDtoPagedResult> Handle(PetListInShelterQuery query, CancellationToken cancellationToken)
        {
            var shelter = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == query.ShelterId, cancellationToken: cancellationToken);

            if (shelter == null)
            {
                throw new BadRequestException("invalid shelter", "Invalid shelter");
            }
            
            var petsFromShelter = _dbContext.Animals.Where(x => x.ShelterId == query.ShelterId)
                .Where(pet => pet.Name.Contains(query.Pagination.SearchText)).ToList();
            
            petsFromShelter = petsFromShelter.Skip((query.Pagination.PageNumber - 1) * query.Pagination.PageSize)
                .Take(query.Pagination.PageSize).ToList();

            var listAnimals = petsFromShelter.Select(pet => new ShelterPetDetailsDto()
                {
                    Id = pet.Id,
                    Name = pet.Name,
                    AnimalCategory = pet.AnimalCategory.ToString(),
                    Species = pet.Species,
                    Gender = pet.Gender,
                    Weight = pet.Weight,
                    ProfilePhoto = pet.ProfilePhoto,
                    Photos = _dbContext.Blobs.Where(x => x.ParentEntityId == pet.Id)
                        .Select(x => x.Id.ToString())
                        .ToList(),
                    Months = pet.Months,
                    CreatedAt = pet.CreatedAt,
                    IsSterilized = pet.IsSterilized,
                    IsVisible = pet.IsVisible,
                    Description = pet.Description
                })
                .ToList();

            listAnimals = Sorter.SortAnimals(listAnimals, query.SortOption, 
                query.Pagination.AscendingSort);

            return new ShelterPetDetailsDtoPagedResult()
            {
                Items = listAnimals,
                ItemsFrom = (query.Pagination.PageNumber - 1) * query.Pagination.PageSize + 1,
                ItemsTo = query.Pagination.PageNumber * query.Pagination.PageSize,
                TotalItemsCount = listAnimals.Count,
                TotalPages = (int)Math.Ceiling((double)listAnimals.Count / query.Pagination.PageSize)
            };
        }
    }
}
