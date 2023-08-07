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
    public record CreatePetCardCommand(string Name, string ProfilePhoto, Genders Gender, string Description, bool IsVisible, int Months, bool IsSterilized, decimal Weight, string Color, AnimalCategories AnimalCategory, Breeds Breed, List<string> Photos,string ShelterId) : IRequest;


    public class CreateCatCardCommandHandler : IRequestHandler<CreatePetCardCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public CreateCatCardCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(CreatePetCardCommand request, CancellationToken cancellationToken)
        {
            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Count; i++)
            {
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            Guid ShelterId;
            try
            {
                ShelterId = new Guid(request.ShelterId);
            }
            catch
            {
                throw new BadRequestException("invalid_Id", "Invalid format of Id");
            }
            var animalCategory = _dbContext.AnimalCategories.First(r => r.CategoryName == request.AnimalCategory.ToString());
            var Shelter = _dbContext.Shelters.FirstOrDefault(r => r.Id == ShelterId);
            if (Shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }
            bool flag = false;
            switch(request.AnimalCategory)
            {
                case AnimalCategories.Cat:
                    if (request.Breed.ToString().StartsWith("Cat") || request.Breed.ToString() == "Inna")
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Dog:
                    if (request.Breed.ToString().StartsWith("Dog") || request.Breed.ToString() == "Inna")
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Rabbit:
                    if (request.Breed.ToString().StartsWith("Rabbit") || request.Breed.ToString().StartsWith("Inna"))
                    {
                        flag = true;
                    }
                    break;
                case AnimalCategories.Undefined:
                    if (!(request.Breed.ToString().StartsWith("Dog") || request.Breed.ToString().StartsWith("Cat") || request.Breed.ToString().StartsWith("Rabbit")))
                    {
                        flag = true;
                    }
                    break;
            }
            if (flag==false)
            {
                throw new BadRequestException("Wrong_Breed", "Trying to assing wrong breed to that animal category");
            }

            Animal newAnimal = new()
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



