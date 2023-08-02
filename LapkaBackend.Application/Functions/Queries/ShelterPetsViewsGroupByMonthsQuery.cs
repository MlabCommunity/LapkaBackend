using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Functions.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    
    public record ShelterPetsViewsGroupByMonthsQuery(string Shelter):IRequest<List<int>>;

    public class ShelterPetsViewsGroupByMonthsQueryHandler : IRequestHandler<ShelterPetsViewsGroupByMonthsQuery, List<int>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public ShelterPetsViewsGroupByMonthsQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByMonthsQuery request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.Shelter);
            var currentYear = DateTime.Now.Year;


            var animalViewsForShelter = await  _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == shelterId && av.ViewDate.Year == currentYear && av.Animal.IsVisible == true)
                .GroupBy(x => x.ViewDate.Month)
                .Select(group => new { Month = group.Key, Count = group.Count() })
                .OrderBy(x => x.Month)
                .ToListAsync();

            var numberOfMonths = 12;
            var result = new List<int>(numberOfMonths);

            for (int i = 1; i <= numberOfMonths; i++)
            {
                var viewsCount = animalViewsForShelter.FirstOrDefault(x => x.Month == i)?.Count ?? 0;
                result.Add(viewsCount);
            }

            return result;
        }
    }

}
