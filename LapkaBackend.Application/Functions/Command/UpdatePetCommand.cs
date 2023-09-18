using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using Serilog;

namespace LapkaBackend.Application.Functions.Command
{
    public record UpdatePetCommand(Guid PetId, string Name, string Species, Genders Gender, string Marking, decimal Weight, string Description,   bool IsSterilized, bool IsVisible, int Months, AnimalCategories AnimalCategory, List<string> Photos):IRequest;


    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IBlobService _blobService;
        private readonly ILogger _logger;

        public UpdatePetCommandHandler(IDataContext dbContext, IBlobService blobService, ILogger logger)
        {
            _dbContext = dbContext;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            var foundAnimal = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId);
            if (foundAnimal is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var animalCategory = await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            if (animalCategory is null)
            {
                throw new BadRequestException("invalid_AnimalCategory", "Animal category doesn't exists");
            }


            for (int i = 0; i < request.Photos.Count; i++)
            {
                if (!string.IsNullOrEmpty(request.Photos[i]))
                {
                    try
                    {
                        var file = _dbContext.Blobs.First(x => x.Id == new Guid(request.Photos[i]));
                        file.ParentEntityId = foundAnimal.Id;
                        file.Index = i;
                        if (i == 0)
                        {
                            foundAnimal.ProfilePhoto = request.Photos[i];
                        }
                    }
                    catch
                    {
                        _logger.Warning("Photo with ID {0} does not exist", request.Photos[i]);
                    }
                }
            }


            foundAnimal.AnimalCategory = animalCategory;
            foundAnimal.Description = request.Description;
            foundAnimal.Gender = request.Gender.ToString();
            foundAnimal.IsSterilized = request.IsSterilized;
            foundAnimal.IsVisible = request.IsVisible;
            foundAnimal.Marking = request.Marking;
            foundAnimal.Months = request.Months;
            foundAnimal.Name = request.Name;
            foundAnimal.Species = request.Species;
            foundAnimal.Weight = request.Weight;

            _dbContext.Animals.Update(foundAnimal);
            await _dbContext.SaveChangesAsync();
        }
    }
        




}
