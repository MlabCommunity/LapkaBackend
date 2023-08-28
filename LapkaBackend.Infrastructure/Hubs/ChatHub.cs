using Microsoft.AspNetCore.SignalR;

namespace LapkaBackend.Infrastructure.Hubs;

public class ChatHub : Hub
{
    private readonly Dictionary<string, string?> _users = new();

    public async Task Test(string msg)
    {
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }
    
    public async Task JoinToDictionary(string userId)
    {
        _users.Add(userId, Context.ConnectionId);

        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "Add to Dictionary");
    }

    public async Task RemoveFromDictionary(string userId)
    {
        _users.Remove(userId);

        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "Remove from Dictionary");
    }

    public async Task CreateGroup(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "Add to group");
    }
}