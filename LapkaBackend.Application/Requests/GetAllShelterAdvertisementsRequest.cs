using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Requests;

public class GetAllShelterAdvertisementsRequest
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public AnimalCategories Type { get; set; } = AnimalCategories.Undefined;
    public Genders Gender { get; set; } = Genders.Undefined;
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Guid UserId { get; set; }
}