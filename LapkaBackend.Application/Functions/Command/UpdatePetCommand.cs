using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdatePetCommand(string PetId, string Description, string Name,Genders Gender, bool IsSterilized, decimal Weight, int Months, List<string> Photos, bool IsVisible, AnimalCategories Category, string Breed, string Marking):IRequest;


    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdatePetCommandHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            Guid petId = new Guid(request.PetId);
            var result = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == petId);

            if (result is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Count; i++)
            {
                if (i==0)
                {
                    photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć
                }
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
           

            var animalCategory = await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.Category.ToString());
            if (animalCategory is null)
            {
                throw new BadRequestException("invalid_AnimalCategory", "Animal category doesn't exists");
            }

            result.AnimalCategory = animalCategory;
            result.Description = request.Description;
            result.Gender = request.Gender.ToString();
            result.IsSterilized = request.IsSterilized;
            result.IsVisible = request.IsVisible;
            result.Marking = request.Marking;
            result.Months = request.Months;
            result.Name = request.Name;
            result.Photos = photosList;
            result.Species = request.Breed.ToString();
            result.Weight = request.Weight;

            _dbContext.Animals.Update(result);
            await _dbContext.SaveChangesAsync();
        }
    }
        




}
