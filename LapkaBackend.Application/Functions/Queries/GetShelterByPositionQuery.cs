using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetShelterByPositionQuery(string Shelter) : IRequest<List<int>>;
    /*
    public class GetShelterByPositionQueryHandler : IRequestHandler<GetShelterByPositionQuery, List<int>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public GetShelterByPositionQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<int>> Handle(GetShelterByPositionQuery request, CancellationToken cancellationToken)
        {
            _dbContext.Shelters.OrderBy

            return result;
        }
    }*/
}
