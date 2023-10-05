using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries
{
    
    public record ShelterPetsViewsGroupByMonthsQuery(Guid ShelterId) : IRequest<List<int>>;

    public class ShelterPetsViewsGroupByMonthsQueryHandler : IRequestHandler<ShelterPetsViewsGroupByMonthsQuery, List<int>>
    {
        private readonly IDataContext _dbContext;

        public ShelterPetsViewsGroupByMonthsQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByMonthsQuery request, CancellationToken cancellationToken)
        {
            const int numberOfMonths = 12;
            var result = new List<int>();
            
            var animalViewsForShelter = await  _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.ShelterId && 
                             av.ViewDate.Year == DateTime.UtcNow.Year && 
                             av.Animal.IsVisible == true)
                .Select(av => new
                { 
                    av.ViewDate.Month
                })
                .ToListAsync(cancellationToken: cancellationToken);
            
            for (var i = 1; i <= numberOfMonths; i++)
            {
                var viewsCount = animalViewsForShelter.Count(x => x.Month == i);
                result.Add(viewsCount);
            }

            return result;
        }
    }

}
