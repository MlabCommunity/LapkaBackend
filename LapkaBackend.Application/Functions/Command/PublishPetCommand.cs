using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record PublishPetCommand(Guid PetId) : IRequest;

    public class PublishPetCommandHandler : IRequestHandler<PublishPetCommand>
    {
        private readonly IDataContext _dbContext;

        public PublishPetCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(PublishPetCommand request, CancellationToken cancellationToken)
        {
            var pet = await _dbContext.Animals
                .FirstOrDefaultAsync(x=>x.Id == request.PetId, cancellationToken: cancellationToken);
            
            if (pet == null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            pet.IsVisible = true;
            _dbContext.Animals.Update(pet);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
