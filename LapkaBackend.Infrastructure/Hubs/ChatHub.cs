using System.Text.RegularExpressions;
using LapkaBackend.Application.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Hubs;

public class ChatHub : Hub
{
    private readonly IDataContext _dataContext;

    public ChatHub(IDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task Test(string msg)
    {
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }
    
    public async Task AddConnectIdToGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        await Clients.Group(userId).SendAsync("CreateGroup", "Add to group");
    }
}