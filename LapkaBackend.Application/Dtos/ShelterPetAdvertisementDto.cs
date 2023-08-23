using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Dtos;

public class ShelterPetAdvertisementDto
{
    [Required]
    public required string OrganizationName { get; set; }
    [Required]
    public required string? OrganizationProfilePicture { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required Guid PetId { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required int Age { get; set; }
    [Required]
    public required bool IsLiked { get; set; }
    [Required]
    public required Genders Gender { get; set; }
    [Required]
    public required string Breed { get; set; }
    [Required]
    public required string ProfilePicture { get; set; }
    [Required]
    public required double Distance { get; set; }
    [Required] 
    public required string City { get; set; }
}