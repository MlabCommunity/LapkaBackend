using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Queries
{
    public record GetPetQuery(Guid PetId, Guid UserId) : IRequest<ShelterPetDetailsDto>;

    public class GetPetQueryHandler : IRequestHandler<GetPetQuery, ShelterPetDetailsDto>
    {
        private readonly IDataContext _dbContext;

        public GetPetQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ShelterPetDetailsDto> Handle(GetPetQuery request, CancellationToken cancellationToken)
        {
            var foundedAnimal = await _dbContext.Animals
                .Include(a => a.AnimalCategory)
                .FirstOrDefaultAsync(x => x.Id == request.PetId, cancellationToken: cancellationToken);

            if (foundedAnimal is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var newAnimalView = new AnimalView()
            {
                Animal = foundedAnimal,
                UserId = request.UserId,
                ViewDate = DateTime.Now
            };

            await _dbContext.AnimalViews.AddAsync(newAnimalView, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var petDto = new ShelterPetDetailsDto()
            {
                Id = foundedAnimal.Id,
                Name = foundedAnimal.Name,
                AnimalCategory = foundedAnimal.AnimalCategory.CategoryName,
                Gender = foundedAnimal.Gender,
                Species = foundedAnimal.Species,
                Weight = foundedAnimal.Weight,
                Photos = _dbContext.Blobs
                    .Where(x => x.ParentEntityId == foundedAnimal.Id)
                    .OrderBy(blob => blob.Index)
                    .Select(blob => blob.Id.ToString())
                    .ToList(),
                Months = foundedAnimal.Months,
                CreatedAt = foundedAnimal.CreatedAt,
                IsSterilized = foundedAnimal.IsSterilized,
                IsVisible = foundedAnimal.IsVisible,
                Description = foundedAnimal.Description
            };

            return petDto;
        }
    }
}
