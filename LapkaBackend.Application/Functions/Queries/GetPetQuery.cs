using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Queries
{
    public record GetPetQuery(Guid PetId, Guid UserId) : IRequest<PetDto>;

    public class GetPetQueryHandler : IRequestHandler<GetPetQuery, PetDto>
    {
        private readonly IDataContext _dbContext;

        public GetPetQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PetDto> Handle(GetPetQuery request, CancellationToken cancellationToken)
        {
            var FoundAnimal = await _dbContext.Animals.Include(a => a.AnimalCategory).FirstOrDefaultAsync(x => x.Id == request.PetId);
            if (FoundAnimal is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var newAnimalView = new AnimalView()
            {
                Animal = FoundAnimal,
                UserId = request.UserId,
                ViewDate = DateTime.Now
            };

            await _dbContext.AnimalViews.AddAsync(newAnimalView); 
            await _dbContext.SaveChangesAsync();

            var petDto = new PetDto()
            {
                PetId = FoundAnimal.Id,
                Name = FoundAnimal.Name,
                AnimalCategory = FoundAnimal.AnimalCategory.CategoryName,
                Gender = FoundAnimal.Gender,
                Species = FoundAnimal.Species,
                Marking = FoundAnimal.Marking,
                Weight = (float)FoundAnimal.Weight,
                ProfilePhoto = FoundAnimal.ProfilePhoto,
                Photos = await _dbContext.Blobs
                            .Where(x => x.ParentEntityId == FoundAnimal.Id)
                            .Select(blob => blob.Id.ToString())
                            .ToArrayAsync(),
                Months = FoundAnimal.Months,
                CreatedAt = FoundAnimal.CreatedAt,
                IsSterilized = FoundAnimal.IsSterilized,
                IsVisible = FoundAnimal.IsVisible,
                Description = FoundAnimal.Description
            };

            return petDto;
        }



    }


    public class PetDto
    {
        public Guid PetId { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Marking { get; set; } = null!;
        public float Weight { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public string AnimalCategory { get; set; } = null!;
        public string? ProfilePhoto { get; set; }
        public string[]? Photos { get; set; }
    }


}
