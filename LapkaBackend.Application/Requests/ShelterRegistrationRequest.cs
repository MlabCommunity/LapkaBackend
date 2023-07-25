using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class ShelterRegistrationRequest
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
