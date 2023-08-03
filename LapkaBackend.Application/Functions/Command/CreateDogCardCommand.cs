using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreateDogCardCommand:IRequest
    {
        public CreateDogCardCommand(DogCard dogCard)
        {
            Name = dogCard.Name;
            ProfilePhoto = dogCard.ProfilePhoto;
            Gender = dogCard.Gender;
            Description = dogCard.Description;
            IsVisible = dogCard.IsVisible;
            Months = dogCard.Months;
            IsSterilized = dogCard.IsSterilized;
            Weight = dogCard.Weight;
            DogColor = dogCard.DogColor;
            DogBreed = dogCard.DogBreed;
            Photos = dogCard.Photos;
            ShelterId = dogCard.ShelterId;
        }

        public string Name { get; set; } = null!;
        public string ProfilePhoto { get; set; } = null!;
        public Gender Gender { get; set; }
        public string Description { get; set; } = null!;
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public string DogColor { get; set; } = null!;
        public string DogBreed { get; set; } = null!;
        public string[] Photos { get; set; } = null!;
        public Guid ShelterId { get; set; }
    }

    public class CreateDogCardCommandHandler : IRequestHandler<CreateDogCardCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public CreateDogCardCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(CreateDogCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Length; i++)
            {
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == AnimalCategories.Dog.ToString());
            var Shelter = _dbContext.Shelters.FirstOrDefault(r => r.Id == request.ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            Animal newAnimal = new()
            {
                Name = request.Name,
                Species = request.DogBreed,
                Gender = request.Gender.ToString(),
                Marking = request.DogColor,
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

    public class DogCard
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string ProfilePhoto { get; set; } = null!;
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public bool IsVisible { get; set; }
        [Required]
        public int Months { get; set; }
        [Required]
        public bool IsSterilized { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public string DogColor { get; set; } = null!;
        [Required]
        public string DogBreed { get; set; } = null!;
        [Required]
        public string[] Photos { get; set; } = null!;
        [Required]
        public Guid ShelterId { get; set; }
        
    }
}

    
