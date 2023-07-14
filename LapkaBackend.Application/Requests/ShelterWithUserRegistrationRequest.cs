using System.ComponentModel.DataAnnotations;
using LapkaBackend.Application.Dtos;

namespace LapkaBackend.Application.Requests
{
    public class ShelterWithUserRegistrationRequest
    {
        [Required]
        public ShelterRegistrationRequest ShelterRequest { get; set; } = null!;
        [Required]
        public UserRegistrationRequest UserRequest { get; set; } = null!;
    }
}
