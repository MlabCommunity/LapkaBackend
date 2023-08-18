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
    public record ShelterPetsViewsGroupByWeeksQuery(Guid Shelter) : IRequest<List<int>>;

    public class ShelterPetsViewsGroupByWeeksQueryHandler : IRequestHandler<ShelterPetsViewsGroupByWeeksQuery, List<int>>
    {
        private readonly IDataContext _dbContext;

        public ShelterPetsViewsGroupByWeeksQueryHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByWeeksQuery request, CancellationToken cancellationToken)
        {
            var currentMonth = DateTime.Now.Month;
            
            var animalViewsForShelter = await _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.Shelter && av.ViewDate.Month == currentMonth && av.Animal.IsVisible == true)
                .Select(av => new
                {
                    DayOfWeek = av.ViewDate.DayOfWeek,
                    AnimalId = av.Animal.Id
                })
                .ToListAsync(); 
            

            var numberOfdays = 7;
            var result = new List<int>();

            for (int i = 1; i <= numberOfdays; i++)
            {
                var value = animalViewsForShelter.Count(x => (int)x.DayOfWeek == i);
                result.Add(value);
            }

            return result;
        }
    }
}
