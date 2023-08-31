using Microsoft.AspNetCore.SignalR;

namespace LapkaBackend.Infrastructure.Hubs;

public class ChatHub : Hub
{
    public async Task AddConnectIdToGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        await Clients.Group(userId).SendAsync("CreateGroup", "Add to group");
    }
}