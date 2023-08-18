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
    public record ShelterPetsViewsGroupByDaysInWeekQuery(Guid Shelter) : IRequest<List<int>>;

    public class ShelterPetsViewsGroupByDaysInWeekQueryHandler : IRequestHandler<ShelterPetsViewsGroupByDaysInWeekQuery, List<int>>
    {
        private readonly IDataContext _dbContext;

        public ShelterPetsViewsGroupByDaysInWeekQueryHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByDaysInWeekQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Now;
            var currentWeekStart = today.AddDays(-(int)today.DayOfWeek); 
            var currentWeekEnd = currentWeekStart.AddDays(7);

            var animalViewsForShelter = await _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.Shelter && av.ViewDate >= currentWeekStart && av.ViewDate < currentWeekEnd && av.Animal.IsVisible==true)
                .Select(av => new
                {
                    DayOfWeek = av.ViewDate.DayOfWeek,
                    ViewDate = av.ViewDate,
                    AnimalId = av.Animal.Id
                })
                .ToListAsync();



            var numberOfDays = 7;
            var result = new List<int>();

            for (int i = 1; i <= numberOfDays; i++)
            {
                var viewsCount = animalViewsForShelter.Count(x => (int)x.DayOfWeek == i);
                result.Add(viewsCount);
            }

            return result;
        }
    }
}
