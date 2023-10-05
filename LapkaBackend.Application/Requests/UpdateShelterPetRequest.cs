using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LapkaBackend.Application.Requests;

public class UpdateShelterPetRequest
{
    [Required]
    public Guid PetId { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public bool IsSterilized { get; set; }
    [Required]
    public decimal Weight { get; set; }
    [Required]
    public int Months { get; set; }
    [Required]
    public Genders Gender { get; set; }
    [Required]
    public List<string> Photos { get; set; }
    [Required]
    public bool IsVisible { get; set; }
}