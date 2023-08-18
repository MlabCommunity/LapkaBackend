namespace LapkaBackend.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public ChatRoom Room { get; set; }
    public User User { get; set; }
    //TODO: Flaga na odczytane i nie odczytane wiadomosći
}