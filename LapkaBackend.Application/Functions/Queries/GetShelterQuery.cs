using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterQuery : IRequest<ShelterDto>
    {
        public GetShelterQuery(Guid shelterId)
        {
            ShelterId = shelterId;
        }

        public Guid ShelterId { get; set; }
    }


    public class GetShelterQueryHandler : IRequestHandler<GetShelterQuery, ShelterDto>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public GetShelterQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<ShelterDto> Handle(GetShelterQuery request, CancellationToken cancellationToken)
        {
            var FoundShelter = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == request.ShelterId);

            var result = _mapper.Map<ShelterDto>(FoundShelter);

            return result;
        }
    }

    public class ShelterDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}