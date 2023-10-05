using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Requests;


namespace LapkaBackend.Application.Functions.Command
{
    public record UpdatePetCommand(UpdateShelterPetRequest Request):IRequest;
    
    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand>
    {
        private readonly IDataContext _dbContext;

        public UpdatePetCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdatePetCommand command, CancellationToken cancellationToken)
        {
            var foundAnimal = await _dbContext.Animals
                .FirstOrDefaultAsync(x => x.Id == command.Request.PetId, cancellationToken: cancellationToken);
            
            if (foundAnimal is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            
            foreach (var file in command.Request.Photos.Select(photo => 
                         _dbContext.Blobs.FirstOrDefault(x => x.Id == new Guid(photo))))
            {
                if (file is null || file.ParentEntityId == foundAnimal.Id)
                {
                    continue;
                }
                
                file.ParentEntityId = foundAnimal.Id;
                _dbContext.Blobs.Update(file);
            }
            
            foundAnimal.Description = command.Request.Description;
            foundAnimal.Gender = command.Request.Gender.ToString();
            foundAnimal.ProfilePhoto = command.Request.Photos.First() == null ? null : command.Request.Photos.First();
            foundAnimal.IsSterilized = command.Request.IsSterilized;
            foundAnimal.IsVisible = command.Request.IsVisible;
            foundAnimal.Months = command.Request.Months;
            foundAnimal.Name = command.Request.Name;
            foundAnimal.Weight = command.Request.Weight;

            _dbContext.Animals.Update(foundAnimal);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
        




}
