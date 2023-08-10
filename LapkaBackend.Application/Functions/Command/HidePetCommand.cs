using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command
{
    public record HidePetCommand(string PetId) : IRequest;

    public class HidePetCommadnHandler : IRequestHandler<HidePetCommand>
    {
        private readonly IDataContext _dbContext;

        public HidePetCommadnHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(HidePetCommand request, CancellationToken cancellationToken)
        {
            Guid petId = new Guid(request.PetId);
            var pet = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == petId);
            if (pet == null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            pet.IsVisible = false;
            _dbContext.Animals.Update(pet);
            await _dbContext.SaveChangesAsync();
        }

    }
}