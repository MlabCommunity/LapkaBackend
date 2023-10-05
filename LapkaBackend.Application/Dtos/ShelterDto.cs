using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Dtos;

public class ShelterDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string OrganizationName { get; set; } = null!;
    [Required]
    public string Email { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string ProfilePhoto { get; set; }
    [Required]
    public string PhoneNumber { get; set; } = null!;
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string Street { get; set; } = null!;
    //TODO: Add distance but talk about how it should look like, because we do not get any of user cords
}