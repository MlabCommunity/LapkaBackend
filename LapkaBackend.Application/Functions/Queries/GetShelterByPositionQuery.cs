using LapkaBackend.Application.Common;
using LapkaBackend.Application.Helper;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterByPositionQuery(float Longitude,float Latitude, int RadiusKm, int PageNumber, int PageSize) : IRequest<ShelterByPositionResponse>;
    
    public class GetShelterByPositionQueryHandler : IRequestHandler<GetShelterByPositionQuery, ShelterByPositionResponse>
    {
        private readonly IDataContext _dbContext;

        public GetShelterByPositionQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<ShelterByPositionResponse> Handle(GetShelterByPositionQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Shelter> allSheltersQuery = _dbContext.Shelters;
            List<Shelter> allShelters = await allSheltersQuery.ToListAsync();// lista wszystkich shronisk

            // sheltersWithRadius - lista zawierająca tylko shroniska znajdujące się w zasięgu, składa się z Shelter i Distance
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

            var sheltersWithRadiusPagination = sheltersWithRadius.Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize);
            var sheltersIdsPagination = sheltersWithRadiusPagination.Select(shelter => shelter.Shelter.Id).ToList();

            var usersWithSheltersPagination = await _dbContext.Users.Include(u=>u.Shelter)
                .Where(user => sheltersIdsPagination.Contains((Guid)user.ShelterId))
                .ToListAsync();

            var usersWithSheltersWithRadiusPagination = usersWithSheltersPagination
            .Select(user => new
            {
                User = user,
                Shelter = user.Shelter,
                Distance = sheltersWithRadius.FirstOrDefault(shelter => shelter.Shelter.Id == user.ShelterId)?.Distance ?? 0.0
            })
            .OrderBy(result => result.Distance)
            .ToList();


            int totalItemsCount = sheltersIds.Count;
            int numberOfPages = (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));


            var sheltersInRadiusDto = usersWithSheltersWithRadiusPagination.Select(p => new SheltersInRadiusDto()
            {
                Id = p.Shelter.Id,
                OrganizationName = p.Shelter.OrganizationName,
                Email = p.User.Email,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                PhoneNumber = p.Shelter.PhoneNumber,
                City = p.Shelter.City,
                Street = p.Shelter.Street,
                Distance = Math.Round(p.Distance, 2)
            })
            .ToList();

            

            ShelterByPositionResponse response = new ShelterByPositionResponse()
            {
                SheltersInRadiusDto = sheltersInRadiusDto,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount
            };

            return response;
        }

        
    }




    public class SheltersInRadiusDto
    {
        public Guid Id { get; set; }
        public string? OrganizationName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //public string? ProfilePhoto { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public double Distance { get; set; }

        public static implicit operator SheltersInRadiusDto(List<SheltersInRadiusDto> v)
        {
            throw new NotImplementedException();
        }
    }

    public class ShelterByPositionResponse
    {
        public List<SheltersInRadiusDto>? SheltersInRadiusDto { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }

}
