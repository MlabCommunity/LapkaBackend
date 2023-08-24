using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command;

public record RemoveLikeToPetCommand(Guid PetId, Guid UserGuid) : IRequest;

public class RemoveLikeToPetCommandHandler : IRequestHandler<RemoveLikeToPetCommand>
{
    private readonly IDataContext _dbContext;

    public RemoveLikeToPetCommandHandler(IDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(RemoveLikeToPetCommand request, CancellationToken cancellationToken)
    {
        var reaction = await _dbContext.Reactions.FirstOrDefaultAsync(x => x.AnimalId == request.PetId 
            && x.UserId == request.UserGuid, cancellationToken: cancellationToken);
        if (reaction == null)
        {
            return;
        }
        
        _dbContext.Reactions.Remove(reaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}