using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreatePetCardCommand(string Name, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string Color, AnimalCategories AnimalCategory, string Breed, string ProfilePhoto, List<string> Photos,Guid ShelterId) : IRequest;


    public class CreateCatCardCommandHandler : IRequestHandler<CreatePetCardCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IBlobService _blobService;
        private readonly IUserService _userService;

        public CreateCatCardCommandHandler(IDataContext dbContext, IBlobService blobService, IUserService userService)
        {

            _dbContext = dbContext;
            _blobService = blobService;
            _userService = userService;
        }

        public async Task Handle(CreatePetCardCommand request, CancellationToken cancellationToken)
        {
            var animalCategory =await _dbContext.AnimalCategories.FirstAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            var Shelter = await _dbContext.Shelters.FirstOrDefaultAsync(r => r.Id == request.ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
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
                Shelter = Shelter
            };

            await _dbContext.Animals.AddAsync(newAnimal);
            await _dbContext.SaveChangesAsync();


            newAnimal.ProfilePhoto = request.ProfilePhoto;
            var fileProfile = _dbContext.Blobs.First(x => x.Id == new Guid(request.ProfilePhoto));
            fileProfile.ParentEntityId = newAnimal.Id;

            for (int i = 0; i < request.Photos.Count; i++)
            {               
                var file = _dbContext.Blobs.First(x => x.Id == new Guid(request.Photos[i]));
                file.ParentEntityId = newAnimal.Id;
            }

            _dbContext.Animals.Update(newAnimal);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class CreatePetCardRequest
    {
        public string Name { get; set; }
        public Genders Gender { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public string Color { get; set; }
        public AnimalCategories AnimalCategory { get; set; }
        public string Breed { get; set; }
        public string ProfilePhoto { get; set; }
        public List<string> Photos { get; set; }
        public Guid ShelterId { get; set; }
    }
}



