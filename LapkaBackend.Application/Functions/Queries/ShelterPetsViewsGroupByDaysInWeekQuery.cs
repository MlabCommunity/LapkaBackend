using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            const int numberOfDays = 7;
            var currentWeekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek); 
            var currentWeekEnd = currentWeekStart.AddDays(7);
            var result = new List<int>();

            var animalViewsForShelter = await _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.Shelter && av.ViewDate >= currentWeekStart && av.ViewDate < currentWeekEnd && av.Animal.IsVisible==true)
                .Select(av => new
                {
                    av.ViewDate.DayOfWeek
                })
                .ToListAsync(cancellationToken: cancellationToken);

            for (var i = 1; i <= numberOfDays; i++)
            {
                var viewsCount = animalViewsForShelter.Count(x => (int)x.DayOfWeek == i);
                result.Add(viewsCount);
            }

            return result;
        }
    }
}
