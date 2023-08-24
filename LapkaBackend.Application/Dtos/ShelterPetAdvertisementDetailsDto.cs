using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Dtos;

public class ShelterPetAdvertisementDetailsDto
{
    [Required]
    public string OrganizationName { get; set; } = null!;
    [Required]
    public string? OrganizationProfilePicture { get; set; }
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public Guid PetId { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public int Age { get; set; }
    [Required]
    public bool IsLiked { get; set; }
    [Required]
    public Genders Gender { get; set; }
    [Required]
    public string Breed { get; set; } = null!;
    [Required]
    public string Color { get; set; } = null!;
    [Required]
    public string ProfilePicture { get; set; } = null!;
    [Required]
    public double Distance { get; set; }
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public bool IsSterilized { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public List<Guid> Photos { get; set; } = new();
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public string Street { get; set; } = null!;
}