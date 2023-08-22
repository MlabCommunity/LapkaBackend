namespace LapkaBackend.Application.Common;

public interface IChatHubContext
{
    Task SendMessageToClient(string message);
}