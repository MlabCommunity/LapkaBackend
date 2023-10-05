namespace LapkaBackend.Domain.Entities
{
    public class Shelter
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public double Longitude { get; set; } 
        public double Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? BankAccount { get; set; }
        public virtual List<Animal> Animals { get; set; }
        public virtual ShelterVolunteering? ShelterVolunteering { get; set; }
    }
}
