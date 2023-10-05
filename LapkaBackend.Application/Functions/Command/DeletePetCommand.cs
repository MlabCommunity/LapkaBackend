using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record DeletePetCommand(Guid PetId) :IRequest;


    public class DeletePetCommandHandler : IRequestHandler<DeletePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IBlobService _blobService;

        public DeletePetCommandHandler(IDataContext dbContext, IBlobService blobService)
        {

            _dbContext = dbContext;
            _blobService = blobService;
        }

        public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Animals
                .FirstOrDefaultAsync(x => x.Id == request.PetId, cancellationToken: cancellationToken);
            
            if (result is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var photos = await _dbContext.Blobs
                .Where(x => x.ParentEntityId == request.PetId)
                .Select(blob => blob.Id.ToString())
                .ToListAsync(cancellationToken: cancellationToken);
            
            await _blobService.DeleteListOfFiles(photos);
            _dbContext.Animals.Remove(result);
            await _dbContext.SaveChangesAsync(cancellationToken);       
        }
    }



}
