namespace LapkaBackend.Application.Common;

public interface IChatHubContext
{
    Task OnConnectedAsync();
    Task OnDisconnectedAsync(Exception e);
}