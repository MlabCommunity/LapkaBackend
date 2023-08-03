using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Helper;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterByPositionQuery(float Longitude,float Latitude, int RadiusKm) : IRequest<List<Shelter>>;
    
    public class GetShelterByPositionQueryHandler : IRequestHandler<GetShelterByPositionQuery, List<Shelter>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public GetShelterByPositionQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<Shelter>> Handle(GetShelterByPositionQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Shelter> allShelters = _dbContext.Shelters.ToList();

            var sheltersInRadius = allShelters
                .Select(shelter =>
                {
                    double distance = DistanceCalculator.CalculateDistance(request.Latitude, request.Longitude, shelter.Latitude, shelter.Longitude);
                    return new { Shelter = shelter, Distance = distance };
                })
                .Where(result => result.Distance <= request.RadiusKm)
                .OrderBy(result => result.Distance)
                .Select(result => result.Shelter)
                .ToList();

            //return sheltersInRadius;


            //
            int totalItemsCount = sheltersInRadius.Count();


            int numberOfPages = (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            var FoundAnimals = _dbContext.Animals
                .Include(a => a.Photos).Include(a => a.AnimalCategory)
                .Where(a => a.IsVisible)
                .OrderBy(x => x.Name)
                .Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize)
                .ToList();

            var PetsList = FoundAnimals.Select(p => new PetInListDto()
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Breed = p.Species,
                Color = p.Marking,
                Weight = (float)p.Weight,
                //ProfilePhoto = p.Photos.FirstOrDefault(p => p.IsProfilePhoto = true),
                //Photos = p.Photos.FindAll(p => p.IsProfilePhoto != true)
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount

            });
        }
    }

    public class SheltersInRadiusDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhoto { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public float Distance { get; set; }
    }

}
