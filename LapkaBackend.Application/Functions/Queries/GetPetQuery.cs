using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Queries
{
    public record GetPetQuery(Guid Id) : IRequest<PetDto>;

    public class GetPetQueryHandler : IRequestHandler<GetPetQuery, PetDto>
    {
        private readonly IDataContext _dbContext;

        public GetPetQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PetDto> Handle(GetPetQuery request, CancellationToken cancellationToken)
        {
            var FoundAnimal = await _dbContext.Animals.Include(a => a.Photos).Include(a => a.AnimalCategory).FirstOrDefaultAsync(x => x.Id == request.Id && x.IsVisible);
            if (FoundAnimal is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var newAnimalView = new AnimalView()
            {
                Animal = FoundAnimal,
                ViewDate=DateTime.Now
            };

            await _dbContext.AnimalViews.AddAsync(newAnimalView);

            await _dbContext.SaveChangesAsync();

            var petDto = new PetDto()
            {
                Id = FoundAnimal.Id,
                Name = FoundAnimal.Name,
                Type = FoundAnimal.AnimalCategory.CategoryName,
                Gender = FoundAnimal.Gender,
                Breed = FoundAnimal.Species,
                Color = FoundAnimal.Marking,
                Weight = (float)FoundAnimal.Weight,
                ProfilePhoto = FoundAnimal.Photos.FirstOrDefault(p => p.IsProfilePhoto = true).Id.ToString(),
                Photos = FoundAnimal.Photos.Where(photo => !photo.IsProfilePhoto).Select(photo => photo.Id.ToString()).ToArray(),
                Age = FoundAnimal.Months,
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
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Color { get; set; } = null!;
        public float Weight { get; set; }
        public string ProfilePhoto { get; set; } = null!;
        public string[] Photos { get; set; } = null!;
        public int Age { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; } = null!;

    }


}
