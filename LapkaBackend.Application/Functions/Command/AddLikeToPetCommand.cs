using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Functions.Command;

public record AddLikeToPetCommand(Guid PetId, Guid UserGuid) : IRequest;

public class AddLikeToPetCommandHandler : IRequestHandler<AddLikeToPetCommand>
{
    private readonly IDataContext _dbContext;

    public AddLikeToPetCommandHandler(IDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(AddLikeToPetCommand request, CancellationToken cancellationToken)
    {
        var reactionExists = await _dbContext.Reactions.FirstOrDefaultAsync(x => x.AnimalId == request.PetId 
            && x.UserId == request.UserGuid, cancellationToken: cancellationToken);
        if (reactionExists != null)
        {
            return;
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.UserGuid, cancellationToken: cancellationToken);
        if (user == null)
        {
            throw new BadRequestException("invalid_user", "User doesn't exists");
        }
        var animal = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId, cancellationToken: cancellationToken);
        if (animal == null)
        {
            throw new BadRequestException("invalid_pet", "Pet doesn't exists");
        }
        var reaction = new Reaction
        {
            User = user,
            UserId = user.Id,
            Animal = animal,
            AnimalId = animal.Id,
        };
        await _dbContext.Reactions.AddAsync(reaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}