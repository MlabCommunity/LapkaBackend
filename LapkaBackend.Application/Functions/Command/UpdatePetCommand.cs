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
    public record UpdatePetCommand(Guid PetId, string Description, string Name,Genders Gender, bool IsSterilized, decimal Weight, int Months, string ProfilePhoto, List<string> Photos, bool IsVisible, AnimalCategories Category, string Breed, string Marking):IRequest;


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

            var Photos = await _dbContext.Blobs.Where(x => x.ParentEntityId == request.PetId).Select(blob => blob.ParentEntityId.ToString()).ToListAsync();
            await _blobService.DeleteListOfFiles(Photos);

            


            var animalCategory = await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.Category.ToString());
            if (animalCategory is null)
            {
                throw new BadRequestException("invalid_AnimalCategory", "Animal category doesn't exists");
            }

            result.ProfilePhoto = request.ProfilePhoto;
            var fileProfile = _dbContext.Blobs.First(x => x.Id == new Guid(request.ProfilePhoto));
            fileProfile.ParentEntityId = result.Id;

            for (int i = 0; i < request.Photos.Count; i++)
            {
                var file = _dbContext.Blobs.First(x => x.Id == new Guid(request.Photos[i]));
                file.ParentEntityId = result.Id;
            }

            result.AnimalCategory = animalCategory;
            result.Description = request.Description;
            result.Gender = request.Gender.ToString();
            result.IsSterilized = request.IsSterilized;
            result.IsVisible = request.IsVisible;
            result.Marking = request.Marking;
            result.Months = request.Months;
            result.Name = request.Name;
            result.Species = request.Breed.ToString();
            result.Weight = request.Weight;

            _dbContext.Animals.Update(result);
            await _dbContext.SaveChangesAsync();
        }
    }
        




}
