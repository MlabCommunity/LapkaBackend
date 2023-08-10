using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Queries
{
    public record PetListInShelterQuery(string ShelterId, int PageNumber, int PageSize) : IRequest<PetListInShelterResponse>;

    public class PetListInShelterQueryHandler : IRequestHandler<PetListInShelterQuery, PetListInShelterResponse>
    {
        private readonly IDataContext _dbContext;

        public PetListInShelterQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PetListInShelterResponse> Handle(PetListInShelterQuery request, CancellationToken cancellationToken)
        {
            Guid ShelterId = new Guid(request.ShelterId);
            int totalItemsCount = await _dbContext.Animals.Where(x => x.ShelterId == ShelterId).CountAsync();
            int numberOfPages =  (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            var FoundAnimals = await _dbContext.Animals
                .Include(a => a.Photos).Include(a => a.AnimalCategory)
                .Where(a => a.IsVisible)
                .Where(a => a.ShelterId == ShelterId)
                .OrderBy(x => x.Name)
                .Skip(request.PageSize * (request.PageNumber-1)).Take(request.PageSize)
                .ToListAsync();

            var petsList = FoundAnimals.Select(p => new PetInListInShelterDto()
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Breed = p.Species,
                Color = p.Marking,
                Weight = (float)p.Weight,
                ProfilePhoto = p.Photos.FirstOrDefault(p => p.IsProfilePhoto = true).Id.ToString(),
                Photos = p.Photos.Where(photo => !photo.IsProfilePhoto).Select(photo => photo.Id.ToString()).ToArray(),
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                

            })
            .ToList();

            var petListResponse = new PetListInShelterResponse()
            {
                PetInListInShelterDto = petsList,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount
            };


            return petListResponse;
        }
    }



    public class PetInListInShelterDto
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
    }

    public class PetListInShelterResponse
    {
        public List<PetInListInShelterDto>? PetInListInShelterDto { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }
}
