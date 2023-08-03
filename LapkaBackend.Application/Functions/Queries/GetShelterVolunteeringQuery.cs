using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterVolunteeringQuery(string shelterId):IRequest<ShelterVolunteeringDto>;

    public class GetShelterVolunteeringQueryHandler : IRequestHandler<GetShelterVolunteeringQuery, ShelterVolunteeringDto>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public GetShelterVolunteeringQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ShelterVolunteeringDto> Handle(GetShelterVolunteeringQuery request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.shelterId);
            var shelterVolunteering = _dbContext.SheltersVolunteering.FirstOrDefault(x => x.ShelterId == shelterId);
            if (shelterVolunteering is null)
            {
                throw new BadRequestException("invalid_Shelter", "Shelter doesn't exists");
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
