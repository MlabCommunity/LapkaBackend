using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests;

public class UpdateShelterRequest
{
    [Required]
    public double Longitude { get; set; }
    [Required]
    public double Latitude { get; set; }
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string Street { get; set; } = null!;
    [Required]
    public string ZipCode { get; set; } = null!;
    [Required]
    public string OrganizationName { get; set; } = null!;
    [Required]
    public string PhoneNumber { get; set; } = null!;
    [Required]
    public string Krs { get; set; } = null!;
    [Required]
    public string Nip { get; set; } = null!;
}