using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Requests;

public class GetAllShelterAdvertisementsRequest
{
    public AnimalCategories Type { get; set; } = AnimalCategories.Undefined;
    public Genders Gender { get; set; } = Genders.Undefined;
    public SortAdvertisementOptions SortOption { get; set; } = SortAdvertisementOptions.Distance;
    
}