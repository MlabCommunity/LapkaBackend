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

namespace LapkaBackend.Application.Functions.Posts
{
    public record CreateDogCartCommand:IRequest
    {
        private DogCard dogCard;

        public CreateDogCartCommand(DogCard dogCard)
        {
            this.dogCard = dogCard;
        }

        public string Name { get; set; } = null!;
        public string PetIdentifier { get; set; } = null!;
        public string ProfilePhoto { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public string DogColor { get; set; } = null!;
        public string DogBreed { get; set; } = null!;
        public string[] Photos { get; set; } = null!;
    }

    public class CreateDogCartCommandHandler : IRequestHandler<CreateDogCartCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public CreateDogCartCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(CreateDogCartCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();

            for (int i = 0; i < request.Photos.Length; i++)
            {
                photosList.Add(new Photo { FilePath = request.Photos[i] });
            }

            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == AnimalCategories.Dog.ToString());

            Animal newAnimal = new()
            {
                Name = request.Name,
                Species = request.DogBreed,
                Gender = request.Gender,
                Marking = request.DogColor,
                Weight = request.Weight,
                ProfilePhoto = request.ProfilePhoto,
                Description = request.Description,
                IsSterilized = request.IsSterilized,
                IsVisible = request.IsVisible,
                Months = request.Months,
                AnimalCategory = animalCategory,
                Photos = photosList,





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
        public string PetIdentifier { get; set; } = null!;
        [Required]
        public string ProfilePhoto { get; set; } = null!;
        [Required]
        public string Gender { get; set; } = null!;
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
    }
}

    
