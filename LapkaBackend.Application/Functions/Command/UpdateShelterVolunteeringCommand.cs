using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterVolunteeringCommand(Guid ShelterId,string BankAccountNumber,string DonationDescription,string DailyHelpDescription,string  TakingDogsOutDescription,bool IsDonationActive,bool IsDailyHelpActive,bool IsTakingDogsOutActive) :IRequest;
    public class UpdateShelterVolunteeringCommandHandler : IRequestHandler<UpdateShelterVolunteeringCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdateShelterVolunteeringCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateShelterVolunteeringCommand request, CancellationToken cancellationToken)
        {
            var shelterVolunteering = await _dbContext.SheltersVolunteering.FirstOrDefaultAsync(x => x.ShelterId == request.ShelterId);

            if (shelterVolunteering is null)
            {
                    throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");                 
            }
            else
            {
                shelterVolunteering.BankAccountNumber = request.BankAccountNumber;
                shelterVolunteering.DailyHelpDescription = request.DailyHelpDescription;
                shelterVolunteering.DonationDescription = request.DonationDescription;
                shelterVolunteering.IsDailyHelpActive = request.IsDailyHelpActive;
                shelterVolunteering.IsDonationActive = request.IsDonationActive;
                shelterVolunteering.IsTakingDogsOutActive = request.IsTakingDogsOutActive;
                shelterVolunteering.TakingDogsOutDesctiption = request.TakingDogsOutDescription;

                _dbContext.SheltersVolunteering.Update(shelterVolunteering);
                await _dbContext.SaveChangesAsync();
            }
            
        }
    }

    public class UpdateShelterVolunteeringRequest
    {
        public string? BankAccountNumber { get; set; }
        public string? DonationDescription { get; set; }
        public string? DailyHelpDescription { get; set; }
        public string? TakingDogsOutDescription { get; set; }
        public bool IsDonationActive { get; set; }
        public bool IsDailyHelpActive { get; set; }
        public bool IsTakingDogsOutActive { get; set; }
    }

}
