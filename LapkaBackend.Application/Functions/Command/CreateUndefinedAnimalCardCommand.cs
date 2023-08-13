using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreateUndefinedAnimalCardCommand(string Name, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string[] Photos, string ShelterId) : IRequest;

    public class CreateUndefinedAnimalCardCommandHandler : IRequestHandler<CreateUndefinedAnimalCardCommand>
    {
        private readonly IDataContext _dbContext;

        public CreateUndefinedAnimalCardCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(CreateUndefinedAnimalCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Length; i++)
            {
                if (i == 0)
                    photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć
                else
                    photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
           

            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == AnimalCategories.Undefined.ToString());
            Guid shelterId = new Guid(request.ShelterId);
            var Shelter = await _dbContext.Shelters.FirstOrDefaultAsync(r => r.Id == shelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            Animal newAnimal = new()
            {
                Name = request.Name,
                Gender = request.Gender.ToString(),
                Weight = request.Weight,
                Description = request.Description,
                IsSterilized = request.IsSterilized,
                IsVisible = request.IsVisible,
                Months = request.Months,
                AnimalCategory = animalCategory,
                Photos = photosList,
                Shelter = Shelter
            };

            await _dbContext.Animals.AddAsync(newAnimal);
            await _dbContext.SaveChangesAsync();
        }
    }

}




