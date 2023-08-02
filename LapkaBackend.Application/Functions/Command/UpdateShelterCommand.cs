using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Functions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Command
{
    public record UpdateShelterCommand:IRequest
    {

        public UpdateShelterCommand(UpdateShelterRequest request, Guid id)
        {
            OrganizationName = request.OrganizationName;
            Longitude = request.Longitude;
            Latitude = request.Latitude;
            City = request.City;
            Street = request.Street;
            ZipCode = request.ZipCode;
            Nip = request.Nip;
            Krs = request.Krs;
            PhoneNumber = request.PhoneNumber;

            ShelterId = id;
        }

        public string OrganizationName { get; set; } = null!;
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public Guid ShelterId { get; set; }
    }

    

    public class UpdateShelterCommandHandler : IRequestHandler<UpdateShelterCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateShelterCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(UpdateShelterCommand request, CancellationToken cancellationToken)
        {
            

            var result = await _dbContext.Shelters.FirstOrDefaultAsync(x => x.Id == request.ShelterId);

            if (result is null)
            {
                throw new BadRequestException("invalid_shelter", "Shelter doesn't exists");
            }

            result.City = request.City;
            result.Krs = request.Krs;
            result.Latitude = request.Latitude;
            result.Longtitude = request.Longitude;
            result.Nip = request.Nip;
            result.OrganizationName = request.OrganizationName;
            result.PhoneNumber = request.PhoneNumber;
            result.Street = request.Street;
            result.ZipCode = request.ZipCode;

            _dbContext.Shelters.Update(result);

            await _dbContext.SaveChangesAsync();
        }
    }


    public class UpdateShelterRequest
    {
        [Required]
        public string OrganizationName { get; set; } = null!;
        [Required]
        public float Longitude { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public string Street { get; set; } = null!;
        [Required]
        public string ZipCode { get; set; } = null!;
        [Required]
        public string Nip { get; set; } = null!;
        [Required]
        public string Krs { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
    }


}
