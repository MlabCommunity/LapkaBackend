using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var result = new List<int>();
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Start of the current month
            var lastDayOfMonth = startDate.AddMonths(1).AddDays(-1); // End of the current month

            
            var animalViewsForShelter = await _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.Shelter && 
                             av.ViewDate.Month == DateTime.UtcNow.Month && 
                             av.Animal.IsVisible == true)
                .Select(av => new
                { 
                    av.ViewDate.Day
                })
                .ToListAsync(cancellationToken: cancellationToken); 
            
                while(startDate <= lastDayOfMonth)
                {
                    var value = animalViewsForShelter.Count(x => x.Day == startDate.Day);
                    result.Add(value);
                    startDate = startDate.AddDays(1);
                }

            return result;
        }
    }
}
