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
    public record GetShelterByPositionQuery(float Longitude,float Latitude, int RadiusKm, int PageNumber, int PageSize) : IRequest<List<Shelter>>;
    
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

            var sheltersWithRadius = allShelters
                .Select(shelter =>
                {
                    double distance = DistanceCalculator.CalculateDistance(request.Latitude, request.Longitude, shelter.Latitude, shelter.Longitude);
                    return new  { Shelter = shelter, Distance = distance };
                })
                .Where(result => result.Distance <= request.RadiusKm)
                .OrderBy(result => result.Distance)
                .ToList();

            var sheltersIds = sheltersWithRadius.Select(shelter => shelter.Shelter.Id).ToList();

            var usersWithShelters = _dbContext.Users.Include(u=>u.Shelter)
                .Where(user => sheltersIds.Contains(user.ShelterId))
                .ToList();

            var usersWithSheltersWithRadius = usersWithShelters
            .Select(user => new
            {
                User = user,
                Shelter = user.Shelter,
                Distance = sheltersWithRadius.FirstOrDefault(shelter => shelter.Shelter.Id == user.ShelterId)?.Distance ?? 0.0
            })
            .OrderBy(result => result.Distance)
            .ToList();





            var PetsList = usersWithSheltersWithRadius.Select(p => new SheltersInRadiusDto()
            {
                Id = p.Shelter.Id,
                OrganizationName = p.Shelter.OrganizationName,
                Email = p.User.Email,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                PhoneNumber = p.Shelter.PhoneNumber,
                City = p.Shelter.City,
                Street = p.Shelter.Street,
                Distance = p.Distance
            });

            int totalItemsCount = sheltersIds.Count;
            int numberOfPages = (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            

            //var PetsList = FoundAnimals.Select(p => new PetInListDto()
            //{
            //    Id = p.Id,
            //    Name = p.Name,
            //    Type = p.AnimalCategory.CategoryName,
            //    Gender = p.Gender,
            //    Breed = p.Species,
            //    Color = p.Marking,
            //    Weight = (float)p.Weight,
            //    //ProfilePhoto = p.Photos.FirstOrDefault(p => p.IsProfilePhoto = true),
            //    //Photos = p.Photos.FindAll(p => p.IsProfilePhoto != true)
            //    Months = p.Months,
            //    CreatedAt = p.CreatedAt,
            //    IsSterilized = p.IsSterilized,
            //    Description = p.Description,
            //    TotalPages = numberOfPages,
            //    TotalItemsCount = totalItemsCount

            //});
        }
    }

    public class SheltersInRadiusDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhoto { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double Distance { get; set; }
    }



}
