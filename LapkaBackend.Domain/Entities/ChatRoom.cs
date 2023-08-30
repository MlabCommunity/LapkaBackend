namespace LapkaBackend.Domain.Entities;

public class ChatRoom
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public virtual User User1 { get; set; }
    public virtual User User2 { get; set; }
    public virtual List<ChatMessage> Messages { get; set; }
}