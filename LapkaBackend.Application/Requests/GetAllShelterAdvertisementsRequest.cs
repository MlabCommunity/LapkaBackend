using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Requests;

public class GetAllShelterAdvertisementsRequest
{
    public AnimalCategories Type { get; set; } = AnimalCategories.Undefined;
    public Genders Gender { get; set; } = Genders.Undefined;
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
    public SortAdvertisementOptions SortOption { get; set; } = SortAdvertisementOptions.Distance;
    public bool AscendingSort { get; set; } = true;
    // true - ascending, false - descending
    public string? SearchText { get; set; } = "";
}