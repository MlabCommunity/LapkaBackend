using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record PublishPetCommand(string PetId):IRequest;

    public class PublishPetCommandHandler : IRequestHandler<PublishPetCommand>
    {
        private readonly IDataContext _dbContext;

        public PublishPetCommandHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task Handle(PublishPetCommand request, CancellationToken cancellationToken)
        {
            Guid petId = new Guid(request.PetId);
            var pet = await _dbContext.Animals.FirstOrDefaultAsync(x=>x.Id == petId);
            if (pet == null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            pet.IsVisible = true;
            _dbContext.Animals.Update(pet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
