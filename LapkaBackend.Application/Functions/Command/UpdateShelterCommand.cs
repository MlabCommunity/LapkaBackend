using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterCommand(string ShelterId, string OrganizationName,float Longitude, float Latitude, string City, string Street, string ZipCode, string Nip, string Krs, string PhoneNumber):IRequest;

    

    public class UpdateShelterCommandHandler : IRequestHandler<UpdateShelterCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdateShelterCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateShelterCommand request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.ShelterId);

            var result = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == shelterId);

            if (result is null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            result.City = request.City;
            result.Krs = request.Krs;
            result.Latitude = request.Latitude;
            result.Longitude = request.Longitude;
            result.Nip = request.Nip;
            result.OrganizationName = request.OrganizationName;
            result.PhoneNumber = request.PhoneNumber;
            result.Street = request.Street;
            result.ZipCode = request.ZipCode;

            _dbContext.Shelters.Update(result);

            await _dbContext.SaveChangesAsync();
        }
    }


}
