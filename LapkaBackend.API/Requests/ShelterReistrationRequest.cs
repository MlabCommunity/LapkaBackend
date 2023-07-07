using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class ShelterRegistrationRequest
{
    [MinLength(1)]
    public string OrganizationName { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    [MinLength(1)]
    public string City { get; set; }
    [MinLength(1)]
    public string Street { get; set; }
    [MinLength(1)]
    public string ZipCode { get; set; }
    [MinLength(1)]
    public string Nip { get; set; }
    [MinLength(1)]
    public string Krs { get; set; }
    [MinLength(1)]
    public string PhoneNumber { get; set; }
}