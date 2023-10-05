using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Helper;

public class Sorter
{
    public static List<ShelterPetAdvertisementDto> SortAdvertisements(List<ShelterPetAdvertisementDto> unsortedList,
        SortAdvertisementOptions option, bool type)
    {
        var sortedList = type switch
        {
            true => option switch
            {
                SortAdvertisementOptions.Name => unsortedList
                    .OrderBy(x => x.Name)
                    .ToList(),
                SortAdvertisementOptions.Age => unsortedList
                    .OrderBy(x => x.Age)
                    .ToList(),
                SortAdvertisementOptions.Distance => unsortedList
                    .OrderBy(x => x.Distance)
                    .ToList(),
                _ => unsortedList
            },
            false => option switch
            {
                SortAdvertisementOptions.Name => unsortedList
                    .OrderByDescending(x => x.Name)
                    .ToList(),
                SortAdvertisementOptions.Age => unsortedList
                    .OrderByDescending(x => x.Age)
                    .ToList(),
                SortAdvertisementOptions.Distance => unsortedList
                    .OrderByDescending(x => x.Distance)
                    .ToList(),
                _ => unsortedList
            },
        };
        return sortedList;
    }
    
    public static List<ShelterPetDetailsDto> SortAnimals(List<ShelterPetDetailsDto> unsortedList,
        SortAnimalOptions option, bool type)
    {
        var sortedList = type switch
        {
            true => option switch
            {
                SortAnimalOptions.Name => unsortedList
                    .OrderBy(x => x.Name)
                    .ToList(),
                SortAnimalOptions.Gender => unsortedList
                    .OrderBy(x => x.Gender)
                    .ToList(),
                SortAnimalOptions.Weight => unsortedList
                    .OrderBy(x => x.Weight)
                    .ToList(),
                SortAnimalOptions.IsSterilized => unsortedList
                    .OrderBy(x => x.IsSterilized)
                    .ToList(),
                SortAnimalOptions.IsVisible => unsortedList
                    .OrderBy(x => x.IsVisible)
                    .ToList(),
                SortAnimalOptions.Months => unsortedList
                    .OrderBy(x => x.Months)
                    .ToList(),
                _ => unsortedList
            },
            false => option switch
            {
                SortAnimalOptions.Name => unsortedList
                    .OrderByDescending(x => x.Name)
                    .ToList(),
                SortAnimalOptions.Gender => unsortedList
                    .OrderByDescending(x => x.Gender)
                    .ToList(),
                SortAnimalOptions.Weight => unsortedList
                    .OrderByDescending(x => x.Weight)
                    .ToList(),
                SortAnimalOptions.IsSterilized => unsortedList
                    .OrderByDescending(x => x.IsSterilized)
                    .ToList(),
                SortAnimalOptions.IsVisible => unsortedList
                    .OrderByDescending(x => x.IsVisible)
                    .ToList(),
                SortAnimalOptions.Months => unsortedList
                    .OrderByDescending(x => x.Months)
                    .ToList(),
                _ => unsortedList
            },
        };
        return sortedList;
    }
    
    public static List<LikedShelterPetsDto> SortLikedAnimals(List<LikedShelterPetsDto> unsortedList,
        SortLikedAnimalOption option, bool type)
    {
        var sortedList = type switch
        {
            true => option switch
            {
                SortLikedAnimalOption.Name => unsortedList
                    .OrderBy(x => x.Name)
                    .ToList(),
                SortLikedAnimalOption.AnimalCategory => unsortedList
                    .OrderBy(x => x.AnimalCategory)
                    .ToList(),
                SortLikedAnimalOption.LikedCount => unsortedList
                    .OrderBy(x => x.LikesCount)
                    .ToList(),
                _ => unsortedList
            },
            false => option switch
            {
                SortLikedAnimalOption.Name => unsortedList
                    .OrderByDescending(x => x.Name)
                    .ToList(),
                SortLikedAnimalOption.AnimalCategory => unsortedList
                    .OrderByDescending(x => x.AnimalCategory)
                    .ToList(),
                SortLikedAnimalOption.LikedCount => unsortedList
                    .OrderByDescending(x => x.LikesCount)
                    .ToList(),
                _ => unsortedList
            },
        };
        return sortedList;
    }
}