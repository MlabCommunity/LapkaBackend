namespace LapkaBackend.Domain.Entities;

public class Shelter
{
    public Guid Id { get; set; }
    public string OrganizationName { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string Nip { get; set; }
    public string Krs { get; set; }
    public string PhoneNumber { get; set; }
    public virtual User User { get; set; } = null!;
}