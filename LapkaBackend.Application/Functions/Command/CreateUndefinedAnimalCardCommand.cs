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
    public record CreateUndefinedAnimalCardCommand : IRequest
    {
        public CreateUndefinedAnimalCardCommand(UndefinedAnimalCard UndefinedAnimalCard)
        {
            Name = UndefinedAnimalCard.Name;
            ProfilePhoto = UndefinedAnimalCard.ProfilePhoto;
            Gender = UndefinedAnimalCard.Gender;
            Description = UndefinedAnimalCard.Description;
            IsVisible = UndefinedAnimalCard.IsVisible;
            Months = UndefinedAnimalCard.Months;
            IsSterilized = UndefinedAnimalCard.IsSterilized;
            Weight = UndefinedAnimalCard.Weight;
            Photos = UndefinedAnimalCard.Photos;
            ShelterId = UndefinedAnimalCard.ShelterId;
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
        public string[] Photos { get; set; } = null!;
        public Guid ShelterId { get; set; }
    }

    public class CreateUndefinedAnimalCardCommandHandler : IRequestHandler<CreateUndefinedAnimalCardCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public CreateUndefinedAnimalCardCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(CreateUndefinedAnimalCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Length; i++)
            {
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == AnimalCategories.Undefined.ToString());
            var Shelter = _dbContext.Shelters.FirstOrDefault(r => r.Id == request.ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            Animal newAnimal = new()
            {
                Name = request.Name,
                Gender = request.Gender,
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

    public class UndefinedAnimalCard
    {
        [Required]
        public string Name { get; set; } = null!;
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
        public string CatColor { get; set; } = null!;
        [Required]
        public string[] Photos { get; set; } = null!;
        [Required]
        public Guid ShelterId { get; set; }

    }
}




