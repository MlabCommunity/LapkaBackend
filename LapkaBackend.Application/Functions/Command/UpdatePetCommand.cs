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


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdatePetCommand(Guid PetId, string Name, string Species, Genders Gender, string Marking, decimal Weight, string Description,   bool IsSterilized, bool IsVisible, int Months, AnimalCategories AnimalCategory, string ProfilePhoto, List<string> Photos):IRequest;


    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IBlobService _blobService;

        public UpdatePetCommandHandler(IDataContext dbContext, IBlobService blobService)
        {
            _dbContext = dbContext;
            _blobService = blobService;
        }

        public async Task Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId);

            if (result is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            

            var animalCategory = await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            if (animalCategory is null)
            {
                throw new BadRequestException("invalid_AnimalCategory", "Animal category doesn't exists");
            }

            var OldPhotosIds = await _dbContext.Blobs.Where(x => x.ParentEntityId == request.PetId).Select(blob => blob.Id.ToString()).ToListAsync();
            await _blobService.DeleteListOfFiles(OldPhotosIds);

            if (!string.IsNullOrEmpty(request.ProfilePhoto))
            {                              
                try
                {                  
                    var fileProfile = _dbContext.Blobs.First(x => x.Id == new Guid(request.ProfilePhoto));
                    fileProfile.ParentEntityId = result.Id;
                    result.ProfilePhoto = request.ProfilePhoto;
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
                        file.ParentEntityId = result.Id;
                    }
                    catch (Exception)
                    {

                        throw new BadRequestException("invalid_photoId", "Photo doesn't exists");
                    }
                }
            }

            result.AnimalCategory = animalCategory;
            result.Description = request.Description;
            result.Gender = request.Gender.ToString();
            result.IsSterilized = request.IsSterilized;
            result.IsVisible = request.IsVisible;
            result.Marking = request.Marking;
            result.Months = request.Months;
            result.Name = request.Name;
            result.Species = request.Species;
            result.Weight = request.Weight;

            _dbContext.Animals.Update(result);
            await _dbContext.SaveChangesAsync();
        }
    }
        




}
