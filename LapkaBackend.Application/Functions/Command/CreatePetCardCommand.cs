using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record CreatePetCardCommand(CreatePetCardRequest Request, Guid ShelterId) : IRequest;


    public class CreateCatCardCommandHandler : IRequestHandler<CreatePetCardCommand>
    {
        private readonly IDataContext _dbContext;

        public CreateCatCardCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(CreatePetCardCommand command, CancellationToken cancellationToken)
        {
            var animalCategory = await _dbContext.AnimalCategories
                .FirstOrDefaultAsync(r => r.CategoryName == command.Request.AnimalCategory.ToString(), 
                    cancellationToken: cancellationToken);
            
            if (animalCategory == null)
            {
                throw new BadRequestException("invalid_animal_category", "That animal category does not exists");
            }
            
            var shelter = await _dbContext.Shelters
                .FirstOrDefaultAsync(r => r.Id == command.ShelterId, cancellationToken: cancellationToken);
            
            if (shelter == null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            var newAnimal = new Animal
            {
                Id = new Guid(),
                Name = command.Request.Name,
                Species = command.Request.Species,
                Gender = command.Request.Gender.ToString(),
                Weight = command.Request.Weight,
                Description = command.Request.Description,
                IsSterilized = command.Request.IsSterilized,
                IsVisible = command.Request.IsVisible,
                Months = command.Request.Months,
                ProfilePhoto = command.Request.Photos.First() == null ? null : command.Request.Photos.First(),
                AnimalCategory = animalCategory,
                Shelter = shelter,
                CreatedAt = DateTime.Now
            };
            
            await _dbContext.Animals.AddAsync(newAnimal, cancellationToken);
            
            foreach (var file in command.Request.Photos.Select(photo => 
                         _dbContext.Blobs.FirstOrDefault(x => x.Id == new Guid(photo))))
            {
                if (file is null)
                {
                    continue;
                }
                
                file.ParentEntityId = newAnimal.Id;
                _dbContext.Blobs.Update(file);
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}



