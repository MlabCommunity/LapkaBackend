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
    public record CreatePetCardCommand(string Name, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string Marking, AnimalCategories AnimalCategory, string Species, string ProfilePhoto, List<string> Photos, Guid ShelterId) : IRequest;


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
            var animalCategory =await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            if (animalCategory == null)
            {
                throw new BadRequestException("invalid_animal_category", "That animal category does not exists");
            }
            var Shelter = await _dbContext.Shelters.FirstOrDefaultAsync(r => r.Id == request.ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            var newAnimal = new Animal
            {
                Name = request.Name,
                Species = request.Species,
                Gender = request.Gender.ToString(),
                Marking = request.Marking,
                Weight = request.Weight,
                Description = request.Description,
                IsSterilized = request.IsSterilized,
                IsVisible = request.IsVisible,
                Months = request.Months,
                AnimalCategory = animalCategory,
                Shelter = Shelter,
                CreatedAt = DateTime.Now
            };

            await _dbContext.Animals.AddAsync(newAnimal);
            await _dbContext.SaveChangesAsync();


            
            if (!string.IsNullOrEmpty(request.ProfilePhoto))
            {                              
                try
                {                  
                    var fileProfile = _dbContext.Blobs.First(x => x.Id == new Guid(request.ProfilePhoto));
                    fileProfile.ParentEntityId = newAnimal.Id;
                    newAnimal.ProfilePhoto = request.ProfilePhoto;
                }
                catch (Exception)
                {
                    throw new BadRequestException("invalid_photoId", "Photo doesn't exists");
                }             
            }
            

            for (int i = 0; i < request.Photos.Count; i++)
            {
                if (!string.IsNullOrEmpty(request.Photos[i]))
                {
                    try
                    {
                        var file = _dbContext.Blobs.First(x => x.Id == new Guid(request.Photos[i]));
                        file.ParentEntityId = newAnimal.Id;
                    }
                    catch (Exception)
                    {

                        throw new BadRequestException("invalid_photoId", "Photo doesn't exists");
                    }                    
                }                           
            }
            _dbContext.Animals.Update(newAnimal);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class CreatePetCardRequest
    {
        public string Name { get; set; } = null!;
        public Genders Gender { get; set; }
        public string Description { get; set; } = null!;
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public string Marking { get; set; } = null!;
        public AnimalCategories AnimalCategory { get; set; }
        public string Species { get; set; } = null!;
        public string? ProfilePhoto { get; set; }
        public List<string>? Photos { get; set; }
    }
}



