using LapkaBackend.Application.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Hubs;

public class ChatHub : Hub
{
    private readonly Dictionary<string, string?> _users = new();
    private readonly IDataContext _dbContext;
    public ChatHub(IDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Test(string msg)
    {
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }
    
    // TODO: Ask that frontend can send us roomId as parameter finally, \n I have to find the other way to find connection between two users
    // TODO: Separate method to JoinToDictionary where we add actually logged user connectionId to dictionary
    // TODO: And the other one like add to group where we getting roomId and we creating group from participants 
    // TODO: The CreateGroup method can be called when user click on conversation? After click we can get roomId that passed to us
    // TODO: Ask that signalr is used to just active chat it means when both of user are live on chat or it works even when one of them is offline
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
    
    public async Task CreateGroup(string connectionId, string roomId)
    {
        await Groups.AddToGroupAsync(connectionId, roomId);
        
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "Add to group");
    }

}