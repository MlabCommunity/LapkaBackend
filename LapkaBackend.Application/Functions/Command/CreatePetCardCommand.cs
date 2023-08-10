using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreatePetCardCommand(string Name, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string Color, AnimalCategories AnimalCategory, Breeds Breed, List<string> Photos,string ShelterId) : IRequest;


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
            for (int i = 0; i < request.Photos.Count; i++)
            {
                if (i==0)
                    photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć
                else
                    photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            

            Guid ShelterId;
            try
            {
                ShelterId = new Guid(request.ShelterId);
            }
            catch
            {
                throw new BadRequestException("invalid_Id", "Invalid format of Id");
            }
            var animalCategory =await _dbContext.AnimalCategories.FirstAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            var Shelter = await _dbContext.Shelters.FirstOrDefaultAsync(r => r.Id == ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }
            bool flag = false;
            switch(request.AnimalCategory)
            {
                case AnimalCategories.Cat:
                    if (request.Breed.ToString().StartsWith(AnimalCategories.Cat.ToString()) || request.Breed.ToString() == "Inna")
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Dog:
                    if (request.Breed.ToString().StartsWith(AnimalCategories.Dog.ToString()) || request.Breed.ToString() == "Inna")
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Rabbit:
                    if (request.Breed.ToString().StartsWith(AnimalCategories.Dog.ToString()) || request.Breed.ToString().StartsWith("Inna"))
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Undefined:
                    if (!(request.Breed.ToString().StartsWith(AnimalCategories.Undefined.ToString()) && (request.Breed.ToString().StartsWith(AnimalCategories.Cat.ToString()) || request.Breed.ToString().StartsWith(AnimalCategories.Dog.ToString()) || request.Breed.ToString().StartsWith(AnimalCategories.Rabbit.ToString()))))
                    {
                        flag = true;
                    }
                    break;
            }
            if (flag==false)
            {
                throw new BadRequestException("Wrong_Breed", "Trying to assing wrong breed to that animal category");
            }

            var newAnimal = new Animal()
            {
                Name = request.Name,
                Species = request.Breed.ToString(),
                Gender = request.Gender.ToString(),
                Marking = request.Color,
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



