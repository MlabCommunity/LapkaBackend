using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Dtos;

public class ShelterPetDetailsDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string AnimalCategory { get; set; }
    [Required]
    public string Species { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public string ProfilePhoto { get; set; }
    [Required]
    public List<string> Photos { get; set; }
    [Required]
    public int Months { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public bool IsSterilized { get; set; }
    [Required]
    public bool IsVisible { get; set; }
    [Required]
    public string Description { get; set; }
}