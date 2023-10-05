using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record HidePetCommand(Guid PetId) : IRequest;

    public class HidePetCommandHandler : IRequestHandler<HidePetCommand>
    {
        private readonly IDataContext _dbContext;

        public HidePetCommandHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(HidePetCommand request, CancellationToken cancellationToken)
        {
            var pet = await _dbContext.Animals
                .FirstOrDefaultAsync(x => x.Id == request.PetId, cancellationToken: cancellationToken);
            
            if (pet == null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            pet.IsVisible = false;
            _dbContext.Animals.Update(pet);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}