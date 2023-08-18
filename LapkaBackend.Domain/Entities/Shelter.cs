using System.ComponentModel.DataAnnotations.Schema;

namespace LapkaBackend.Domain.Entities
{
    public class Shelter
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public float Longitude { get; set; } 
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public virtual List<Animal> Animals { get; set; }
        public ShelterVolunteering? ShelterVolunteering { get; set; }
    }
}
