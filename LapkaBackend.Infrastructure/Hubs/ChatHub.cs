using System.Security.Claims;
using LapkaBackend.Application.Common;
using Microsoft.AspNetCore.SignalR;

namespace LapkaBackend.Infrastructure.Hubs;

public class ChatHub : Hub, IChatHubContext
{
    private readonly Dictionary<Guid, string> _users = new();

    public override Task OnConnectedAsync()
    {
         _users.Add(new Guid(Context.User.FindFirstValue("userId")), Context.ConnectionId);  
         
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception e)
    {
        _users.Remove(new Guid(Context.User.FindFirstValue("userId")));  
         
        return base.OnDisconnectedAsync(e);
    }
}