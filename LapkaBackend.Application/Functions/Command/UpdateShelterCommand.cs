using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterCommand(Guid ShelterId, string OrganizationName,float Longitude, float Latitude, string City, string Street, string ZipCode, string Nip, string Krs, string PhoneNumber):IRequest;

    public class UpdateShelterCommandHandler : IRequestHandler<UpdateShelterCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdateShelterCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateShelterCommand request, CancellationToken cancellationToken)
        {

            var result = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == request.ShelterId);

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

    public class UpdateShelterRequest
    {
        public Guid ShelterId { get; set; }
        public string OrganizationName { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Nip { get; set; }
        public string Krs { get; set; }
        public string PhoneNumber { get; set; }
    }


}
