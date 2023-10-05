using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries;

public record GetLikedPetsQuery(PaginationDto Pagination, Guid ShelterId, SortLikedAnimalOption SortOption) : IRequest<LikedShelterPetsDtoPagedResult>;

public class GetLikedPetHandler : IRequestHandler<GetLikedPetsQuery, LikedShelterPetsDtoPagedResult>
{
    private readonly IDataContext _dbContext;

    public GetLikedPetHandler(IDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LikedShelterPetsDtoPagedResult> Handle(GetLikedPetsQuery query, CancellationToken cancellationToken)
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

        var listAnimals = petsFromShelter.Select(pet => new LikedShelterPetsDto()
        {
            Id = pet.Id,
            Name = pet.Name,
            AnimalCategory = pet.AnimalCategory.ToString(),
            ProfilePhoto = pet.ProfilePhoto,
            LikesCount = _dbContext.AnimalViews.Count(x => x.AnimalId == pet.Id)
        }).ToList();
        
        listAnimals = Sorter.SortLikedAnimals(listAnimals, query.SortOption, 
            query.Pagination.AscendingSort);
        
        return new LikedShelterPetsDtoPagedResult()
        {
            Items = listAnimals,
            ItemsFrom = (query.Pagination.PageNumber - 1) * query.Pagination.PageSize + 1,
            ItemsTo = query.Pagination.PageNumber * query.Pagination.PageSize,
            TotalItemsCount = listAnimals.Count,
            TotalPages = (int)Math.Ceiling((double)listAnimals.Count / query.Pagination.PageSize)
        };
    }
}