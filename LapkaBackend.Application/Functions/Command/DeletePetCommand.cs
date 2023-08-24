﻿using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace LapkaBackend.Application.Functions.Command
{
    public record DeletePetCommand(Guid PetId) :IRequest;


    public class DeletePetCommandHandler : IRequestHandler<DeletePetCommand>
    {
        private readonly IDataContext _dbContext;

        public DeletePetCommandHandler(IDataContext dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId);
            if (result is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var animalPhotos = await _dbContext.Photos.Where(p => p.AnimalId == result.Id).ToListAsync();
            _dbContext.Photos.RemoveRange(animalPhotos);

            _dbContext.Animals.Remove(result);
            await _dbContext.SaveChangesAsync();       
        }
    }



}
