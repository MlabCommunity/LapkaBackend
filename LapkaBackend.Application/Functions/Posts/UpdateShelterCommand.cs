using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Posts
{
    public record UpdateShelterCommand:IRequest
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

    public class UpdateShelterCommandHandler : IRequestHandler<UpdateShelterCommand>
    {
        public Task Handle(UpdateShelterCommand request, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
