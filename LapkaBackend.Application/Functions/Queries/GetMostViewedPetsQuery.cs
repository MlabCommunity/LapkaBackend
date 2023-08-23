using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Queries
{
    public record GetMostViewedPetsQuery(Guid ShelterId) : IRequest<List<MostViewedPetsResponse>>;

    public class GetMostViewedPetsQueryHandler : IRequestHandler<GetMostViewedPetsQuery, List<MostViewedPetsResponse>>
    {
        private readonly IDataContext _dbContext;

        public GetMostViewedPetsQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MostViewedPetsResponse>> Handle(GetMostViewedPetsQuery request, CancellationToken cancellationToken)
        {

            var FoundAnimals = await _dbContext.Animals
                .Include(a => a.AnimalViews)
                .Include(a => a.AnimalCategory)
                .Where(a => a.IsVisible)
                .Where(a => a.ShelterId == request.ShelterId)
                .Where(a => a.AnimalViews.Any())
                .OrderByDescending(x => x.AnimalViews.Count())
                .Take(10)
                .ToListAsync();

            var mostViewedPetsResponse = FoundAnimals.Select( p => new MostViewedPetsResponse()
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Breed = p.Species,
                Color = p.Marking,
                Weight = (float)p.Weight,
                ProfilePhoto = p.ProfilePhoto,
                Photos =  _dbContext.Blobs.Where(x => x.ParentEntityId == p.Id).Select(blob => blob.ParentEntityId.ToString()).ToArray(),
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                AnimalViews = p.AnimalViews.Count,


            })
             .ToList();

            return mostViewedPetsResponse;
        }
    }

    public class MostViewedPetsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Color { get; set; } = null!;
        public float Weight { get; set; }
        public string? ProfilePhoto { get; set; }
        public string[]? Photos { get; set; }
        public int Months { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public string Description { get; set; } = null!;
        public int? AnimalViews { get; set; }
    }
}
