using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Command
{
    public record AddPetToArchiveCommand : IRequest
    {
        private string petId;


        public AddPetToArchiveCommand(string petId)
        {
            this.petId = petId;
        }

        public string PetId { get; set; }
    }

    public class AddPetToArchiveCommandHandler : IRequestHandler<AddPetToArchiveCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public AddPetToArchiveCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(AddPetToArchiveCommand request, CancellationToken cancellationToken)
        {
            Guid petId = new Guid(request.PetId);
            var animal = _dbContext.Animals.FirstOrDefault(x => x.Id == petId);

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
