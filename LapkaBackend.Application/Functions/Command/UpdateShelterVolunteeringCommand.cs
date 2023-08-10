using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterVolunteeringCommand(string ShelterId,string BankAccountNumber,string DonationDescription,string DailyHelpDescription,string  TakingDogsOutDescription,bool IsDonationActive,bool IsDailyHelpActive,bool IsTakingDogsOutActive) :IRequest;
    public class UpdateShelterVolunteeringCommandHandler : IRequestHandler<UpdateShelterVolunteeringCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdateShelterVolunteeringCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateShelterVolunteeringCommand request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.ShelterId);
            var shelterVolunteering = await _dbContext.SheltersVolunteering.FirstOrDefaultAsync(x => x.ShelterId == shelterId);

            if (shelterVolunteering is null)
            {
                var shelter = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == shelterId);
                if (shelter is null)
                {
                    throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
                }
                else
                {
                    ShelterVolunteering newShelterVolunteering = new()
                    {
                        ShelterId=shelterId,
                        Shelter = shelter,
                        BankAccountNumber = request.BankAccountNumber,
                        DailyHelpDescription = request.DailyHelpDescription,
                        DonationDescription = request.DonationDescription,
                        IsDailyHelpActive = request.IsDailyHelpActive,
                        IsDonationActive = request.IsDonationActive,
                        IsTakingDogsOutActive = request.IsTakingDogsOutActive,
                        TakingDogsOutDesctiption = request.TakingDogsOutDescription
                    };

                    await _dbContext.SheltersVolunteering.AddAsync(newShelterVolunteering);
                    await _dbContext.SaveChangesAsync();
                }
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

}
