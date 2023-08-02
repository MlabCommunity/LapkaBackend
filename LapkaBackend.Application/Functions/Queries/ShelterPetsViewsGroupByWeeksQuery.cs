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
    public record ShelterPetsViewsGroupByWeeksQuery(string Shelter) : IRequest<List<int>>;

    public class ShelterPetsViewsGroupByWeeksQueryHandler : IRequestHandler<ShelterPetsViewsGroupByWeeksQuery, List<int>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public ShelterPetsViewsGroupByWeeksQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByWeeksQuery request, CancellationToken cancellationToken)
        {
            Guid shelterId = new Guid(request.Shelter);
            var currentMonth = DateTime.Now.Month;

            var animalViewsForShelter = await _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == shelterId && av.ViewDate.Year == currentMonth && av.Animal.IsVisible == true)
                .GroupBy(x => ((int)x.ViewDate.DayOfWeek))
                .Select(group => new { DayOfWeek = group.Key, Count = group.Count() })
                .OrderBy(x => x.DayOfWeek)
                .ToListAsync();

            var numberOfMonths = 7;
            var result = new List<int>(numberOfMonths);

            for (int i = 1; i <= numberOfMonths; i++)
            {
                var viewsCount = animalViewsForShelter.FirstOrDefault(x => x.DayOfWeek == i)?.Count ?? 0;
                result.Add(viewsCount);
            }

            return result;
        }
    }
}
