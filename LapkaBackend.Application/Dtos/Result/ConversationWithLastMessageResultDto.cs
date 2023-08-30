using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Dtos.Result;

public class ConversationWithLastMessageResultDto
{
    public Guid? RoomId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
}