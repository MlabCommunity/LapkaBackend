using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterQuery(Guid ShelterId) : IRequest<ShelterDto>;

    public class GetShelterQueryHandler : IRequestHandler<GetShelterQuery, ShelterDto>
    {
        private readonly IDataContext _dbContext;

        public GetShelterQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ShelterDto> Handle(GetShelterQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .Include(x => x.Shelter)
                .FirstOrDefaultAsync(x => x.ShelterId == request.ShelterId, cancellationToken: cancellationToken);

            if (result is null)
            {
                throw new NotFoundException("invalid_shelterId", "Shelter does not found");
            }
            
            return new ShelterDto
            {
                Id = result.Id,
                OrganizationName = result.Shelter.OrganizationName,
                Email = result.Email,
                FirstName = result.FirstName,
                LastName = result.LastName,
                ProfilePhoto = result.ProfilePicture,
                PhoneNumber = result.Shelter.PhoneNumber,
                City = result.Shelter.City,
                Street = result.Shelter.Street
            };
        }
    }
}