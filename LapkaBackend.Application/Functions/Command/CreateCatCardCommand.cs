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
    public record CreateCatCardCommand : IRequest
    {
        public CreateCatCardCommand(CatCard CatCard)
        {
            Name = CatCard.Name;
            ProfilePhoto = CatCard.ProfilePhoto;
            Gender = CatCard.Gender;
            Description = CatCard.Description;
            IsVisible = CatCard.IsVisible;
            Months = CatCard.Months;
            IsSterilized = CatCard.IsSterilized;
            Weight = CatCard.Weight;
            CatColor = CatCard.CatColor;
            CatBreed = CatCard.CatBreed;
            Photos = CatCard.Photos;
            ShelterId = CatCard.ShelterId;
        }

        public string Name { get; set; } = null!;
        public string PetIdentifier { get; set; } = null!;
        public string ProfilePhoto { get; set; } = null!;
        public Gender Gender { get; set; }
        public string Description { get; set; } = null!;
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public string CatColor { get; set; } = null!;
        public string CatBreed { get; set; } = null!;
        public string[] Photos { get; set; } = null!;
        public Guid ShelterId { get; set; }
    }

    public class CreateCatCardCommandHandler : IRequestHandler<CreateCatCardCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public CreateCatCardCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(CreateCatCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Length; i++)
            {
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == AnimalCategories.Cat.ToString());
            var Shelter = _dbContext.Shelters.FirstOrDefault(r => r.Id == request.ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            Animal newAnimal = new()
            {
                Name = request.Name,
                Species = request.CatBreed,
                Gender = request.Gender.ToString(),
                Marking = request.CatColor,
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

    public class CatCard
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
        public string CatColor { get; set; } = null!;
        [Required]
        public string CatBreed { get; set; } = null!;
        [Required]
        public string[] Photos { get; set; } = null!;
        [Required]
        public Guid ShelterId { get; set; }

    }
}



