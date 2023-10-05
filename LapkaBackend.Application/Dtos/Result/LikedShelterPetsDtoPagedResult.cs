using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Dtos.Result;

public class LikedShelterPetsDtoPagedResult
{
    [Required]
    public List<LikedShelterPetsDto> Items { get; set; }
    [Required]
    [ReadOnly(true)]
    public int TotalPages { get; set; }
    [Required]
    [ReadOnly(true)]
    public int ItemsFrom { get; set; }
    [Required]
    [ReadOnly(true)]
    public int ItemsTo { get; set; }
    [Required]
    [ReadOnly(true)]
    public int TotalItemsCount { get; set; }
}