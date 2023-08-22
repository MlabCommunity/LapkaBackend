using LapkaBackend.Application.Common;
using Microsoft.AspNetCore.SignalR;

namespace LapkaBackend.Infrastructure.ChatHub;

public class ChatHub : Hub, IChatHubContext
{
    //ConnectionId = current logged user connection
    
    public async Task SendMessageToClient(string message)
    {

        await Groups.AddToGroupAsync(Context.ConnectionId, $"xDD");
        await Clients.Groups("xDD").SendAsync("ReceiveMessage", "xdd", message);
    }
    
}