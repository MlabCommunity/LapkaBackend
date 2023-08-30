using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Queries
{
    
    public record ShelterPetsViewsGroupByMonthsQuery(Guid Shelter):IRequest<List<int>>;

    public class ShelterPetsViewsGroupByMonthsQueryHandler : IRequestHandler<ShelterPetsViewsGroupByMonthsQuery, List<int>>
    {
        private readonly IDataContext _dbContext;

        public ShelterPetsViewsGroupByMonthsQueryHandler(IDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
        }

        public async Task<List<int>> Handle(ShelterPetsViewsGroupByMonthsQuery request, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;


            var animalViewsForShelter = await  _dbContext.AnimalViews
                .Where(av => av.Animal.ShelterId == request.Shelter && av.ViewDate.Year == currentYear && av.Animal.IsVisible == true)
                .Select(av => new
                {
                    Month = av.ViewDate.Month,
                    ViewDate = av.ViewDate,
                    AnimalId = av.Animal.Id
                })
                .ToListAsync();

            var numberOfMonths = 12;
            var result = new List<int>();

            for (int i = 1; i <= numberOfMonths; i++)
            {
                var viewsCount = animalViewsForShelter.Count(x => x.Month == i);
                result.Add(viewsCount);
            }

            return result;
        }
    }

}
