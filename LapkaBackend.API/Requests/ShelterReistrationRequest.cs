using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class ShelterRegistrationRequest
{
    [Required]
    public string OrganizationName { get; set; }
    [Required]
    public double Longitude { get; set; }
    [Required]
    public double Latitude { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Street { get; set; }
    [Required]
    public string ZipCode { get; set; }
    [Required]
    public string Nip { get; set; }
    [Required]
    public string Krs { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
}