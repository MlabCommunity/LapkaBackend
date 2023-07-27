using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetListOfSheltersQuery : IRequest<List<ShelterInListDto>>;


    public class GetListOfSheltersQueryHandler : IRequestHandler<GetListOfSheltersQuery, List<ShelterInListDto>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public GetListOfSheltersQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ShelterInListDto>> Handle(GetListOfSheltersQuery request, CancellationToken cancellationToken)
        {
            var FoundShelters = await _dbContext.Shelters.ToListAsync();

            var result = _mapper.Map<List<ShelterInListDto>>(FoundShelters);

            return result;
        }
    }

    public class ShelterInListDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public float Longtitude { get; set; }
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }


}
