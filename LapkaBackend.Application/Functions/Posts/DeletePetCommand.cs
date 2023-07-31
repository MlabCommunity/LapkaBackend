using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Posts
{
    public record DeletePetCommand(string PetId) :IRequest;


    public class DeletePetCommandHandler : IRequestHandler<DeletePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public DeletePetCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(request.PetId, out Guid id))
            {
                var result = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == id);
                if (result is null)
                {
                    throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
                }

                var animalPhotos = _dbContext.Photos.Where(p => p.AnimalId == result.Id).ToList();
                _dbContext.Photos.RemoveRange(animalPhotos);

                _dbContext.Animals.Remove(result);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("invalid_Id", "Pet doesn't exists");
            }           
        }
    }



}
