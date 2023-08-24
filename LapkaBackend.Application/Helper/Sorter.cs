using LapkaBackend.Application.Dtos;
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
}