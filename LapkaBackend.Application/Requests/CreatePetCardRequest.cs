using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Requests;

public class CreatePetCardRequest
{
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Species { get; set; } = null!;
    [Required]
    public Genders Gender { get; set; }
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public bool IsVisible { get; set; }
    [Required]
    public int Months { get; set; }
    [Required]
    public bool IsSterilized { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public List<string>? Photos { get; set; }
    [Required]
    public AnimalCategories AnimalCategory { get; set; }
}