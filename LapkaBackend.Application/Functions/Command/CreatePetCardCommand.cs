using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreatePetCardCommand(string Name, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string Color, AnimalCategories AnimalCategory, string Breed, string ProfilePhoto, List<string> Photos,string ShelterId) : IRequest;


    public class CreateCatCardCommandHandler : IRequestHandler<CreatePetCardCommand>
    {
        private readonly IDataContext _dbContext;

        public CreateCatCardCommandHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task Handle(CreatePetCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            for (int i = 0; i < request.Photos.Count; i++)
            {
                    photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            

            Guid shelterId;
            try
            {
                shelterId = new Guid(request.ShelterId);
            }
            catch 
            {
                throw new BadRequestException("invalid_Id", "Invalid format of Id");
            }
            var animalCategory = await _dbContext.AnimalCategories
                .FirstAsync(r => r.CategoryName == request.AnimalCategory.ToString(), cancellationToken: cancellationToken);
            var shelter = await _dbContext.Shelters.FirstOrDefaultAsync(r => r.Id == shelterId, cancellationToken: cancellationToken);
            if (shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            var newAnimal = new Animal
            {
                Name = request.Name,
                Species = request.Breed,
                Gender = request.Gender.ToString(),
                Marking = request.Color,
                Weight = request.Weight,
                Description = request.Description,
                IsSterilized = request.IsSterilized,
                IsVisible = request.IsVisible,
                Months = request.Months,
                AnimalCategory = animalCategory,
                Photos = photosList,
                Shelter = shelter
            };
            shelter.Animals.Add(newAnimal);

            await _dbContext.Animals.AddAsync(newAnimal, cancellationToken);
            _dbContext.Shelters.Update(shelter);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }


}



