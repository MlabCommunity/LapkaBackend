using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterCommand(Guid ShelterId, UpdateShelterRequest Request) : IRequest;

    public class UpdateShelterCommandHandler : IRequestHandler<UpdateShelterCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdateShelterCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateShelterCommand command, CancellationToken cancellationToken)
        {

            var result = await _dbContext.Shelters
                .FirstOrDefaultAsync(x => x.Id == command.ShelterId, cancellationToken: cancellationToken);

            if (result is null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            result.City = command.Request.City;
            result.Krs = command.Request.Krs;
            result.Latitude = command.Request.Latitude;
            result.Longitude = command.Request.Longitude;
            result.Nip = command.Request.Nip;
            result.OrganizationName = command.Request.OrganizationName;
            result.PhoneNumber = command.Request.PhoneNumber;
            result.Street = command.Request.Street;
            result.ZipCode = command.Request.ZipCode;

            _dbContext.Shelters.Update(result);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    


}
