using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace LapkaBackend.Application.Dtos.Result;

public class ShelterPetDetailsDtoPagedResult
{
    [Required]
    public List<ShelterPetDetailsDto> Items { get; set; } = new();
    [Required]
    [SwaggerSchema(ReadOnly = true)]
    public int TotalPages { get; set; }
    [Required]
    [ReadOnly(true)]
    [SwaggerSchema(ReadOnly = true)]
    public int ItemsFrom { get; set; }
    [Required]
    [ReadOnly(true)]
    [SwaggerSchema(ReadOnly = true)]
    public int ItemsTo { get; set; }
    [Required]
    [ReadOnly(true)]
    [SwaggerSchema(ReadOnly = true)]
    public int TotalItemsCount { get; set; }
    
}