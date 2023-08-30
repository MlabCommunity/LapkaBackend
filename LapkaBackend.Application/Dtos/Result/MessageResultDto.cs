namespace LapkaBackend.Application.Dtos.Result;

public class MessageResultDto
{
    public string Content { get; set; }
    public ChatUserDto User { get; set; }
    public DateTime Date { get; set; }
}