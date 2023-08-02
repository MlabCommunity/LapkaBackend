using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Command
{
    public record HidePetCommand(string petId) : IRequest;

    public class HidePetCommadnHandler : IRequestHandler<HidePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public HidePetCommadnHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(HidePetCommand request, CancellationToken cancellationToken)
        {
            Guid petId = new Guid(request.petId);
            var pet = _dbContext.Animals.FirstOrDefault(x => x.Id == petId);
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