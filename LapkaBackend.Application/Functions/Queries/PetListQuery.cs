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
    public record PetListQuery(int PageNumber, int PageSize) : IRequest<List<PetInListDto>>;

    public class PetListCommandHandler : IRequestHandler<PetListQuery, List<PetInListDto>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public PetListCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<PetInListDto>> Handle(PetListQuery request, CancellationToken cancellationToken)
        {
            
            //elements / request.pageSize = numberOfPages
            var FoundAnimals = _dbContext.Animals.OrderBy(x => x.Name).Skip(request.PageSize * (request.PageNumber-1)).Take(request.PageSize).ToList();
            //var PetInListDto = _mapper.Map<List<PetInListDto>>(FoundAnimals);
            mapper.Map<Source, Dest>(src, opt => {
                opt.BeforeMap((src, dest) => src.Value = src.Value + i);
                opt.AfterMap((src, dest) => dest.Name = HttpContext.Current.Identity.Name);

                var PetInListDto = _mapper.Map<List<PetInListDto>>(FoundAnimals, opts => opts.AfterMap => PetInListDto. = HttpContext.Current.Identity.Name = request.PageNumber);
            return PetInListDto;
        }
    }

    public class PetInListDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Color { get; set; } = null!;
        public float Weight { get; set; }
        public string ProfilePhoto { get; set; } = null!;
        public string Photos { get; set; } = null!;
        public int Months { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public string Description { get; set; } = null!;

        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }
}
