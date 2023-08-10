using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterVolunteeringQuery(string ShelterId):IRequest<ShelterVolunteeringDto>;

    public class GetShelterVolunteeringQueryHandler : IRequestHandler<GetShelterVolunteeringQuery, ShelterVolunteeringDto>
    {
        private readonly IDataContext _dbContext;

        public GetShelterVolunteeringQueryHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task<ShelterVolunteeringDto> Handle(GetShelterVolunteeringQuery request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.ShelterId);
            var shelterVolunteering = await _dbContext.SheltersVolunteering.FirstOrDefaultAsync(x => x.ShelterId == shelterId);
            if (shelterVolunteering is null)
            {
                throw new BadRequestException("invalid_Shelter", "Shelter doesn't exists or does not have voluntering bookmark");
            };

            var shelterVolunteeringDto = new ShelterVolunteeringDto()
            {
                IsDonationActive = shelterVolunteering.IsDonationActive,
                BankAccountNumber = shelterVolunteering.BankAccountNumber,
                DonationDescription = shelterVolunteering.DonationDescription,
                IsDailyHelpActive = shelterVolunteering.IsDailyHelpActive,
                DailyHelpDescription = shelterVolunteering.DailyHelpDescription,
                IsTakingDogsOutActive = shelterVolunteering.IsTakingDogsOutActive,
                TakingDogsOutDesctiption = shelterVolunteering.TakingDogsOutDesctiption
            };

            return shelterVolunteeringDto;
        }
    }

    public class ShelterVolunteeringDto
    {
        public bool IsDonationActive { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? DonationDescription { get; set; }
        public bool IsDailyHelpActive { get; set; }
        public string? DailyHelpDescription { get; set; }
        public bool IsTakingDogsOutActive { get; set; }
        public string? TakingDogsOutDesctiption { get; set; }
    }

}
