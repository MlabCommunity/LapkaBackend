using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Dtos;

public class LikedShelterPetsDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string AnimalCategory { get; set; }
    public string ProfilePhoto { get; set; }
    public int LikesCount { get; set; }
}