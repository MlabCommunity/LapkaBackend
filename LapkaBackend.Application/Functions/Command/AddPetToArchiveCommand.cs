using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record AddPetToArchiveCommand(Guid PetId) : IRequest;

    public class AddPetToArchiveCommandHandler : IRequestHandler<AddPetToArchiveCommand>
    {
        private readonly IDataContext _dbContext;

        public AddPetToArchiveCommandHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task Handle(AddPetToArchiveCommand request, CancellationToken cancellationToken)
        {
            var animal = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId);

            if (animal == null)
            {
                throw new BadRequestException("invalid_pet", "Pet doesn't exists");
            }
            animal.IsArchival = true;

            _dbContext.Animals.Update(animal);
            await _dbContext.SaveChangesAsync();
        }
    }
}
