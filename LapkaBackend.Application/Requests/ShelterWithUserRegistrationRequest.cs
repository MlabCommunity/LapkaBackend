using System.ComponentModel.DataAnnotations;

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
