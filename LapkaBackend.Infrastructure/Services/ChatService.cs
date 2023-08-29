using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IDataContext _dbContext;

    public ChatService(IHubContext<ChatHub> chatHubContext, IDataContext dbContext)
    {
        _chatHubContext = chatHubContext;
        _dbContext = dbContext;
    }


    public async Task SendMessage(string message, Guid sender, Guid receiver)
    {
        if (await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == receiver) is null)
        {
            throw new BadRequestException("invalid_id", "Receiver is invalid");
        }
            
        var room = await _dbContext.ChatRooms
            .FirstOrDefaultAsync(x => 
                (x.User1Id == sender && x.User2Id == receiver) || 
                (x.User1Id == receiver && x.User2Id == sender));
        
        if (room is null)
        {
            room = new ChatRoom()
            {
                User1Id = sender,
                User2Id = receiver
            };

            await _dbContext.ChatRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
        }
        
        var newMessage = new ChatMessage
        {
            RoomId = room.Id,
            UserId = sender,
            Content = message,
            Date = DateTime.UtcNow,
            IsRead = false
        };
            
        await _dbContext.ChatMessages.AddAsync(newMessage);
        await _dbContext.SaveChangesAsync();
        
        // Send notification about new message after called JoinToDictionary method in ChatHub
        await _chatHubContext.Clients.Group(room.User1Id.ToString()).SendAsync("NotifyNewMessage", $"{room.Id}");
        await _chatHubContext.Clients.Group(room.User2Id.ToString()).SendAsync("NotifyNewMessage", $"{room.Id}");
    }

    public Task<List<ConversationWithLastMessageResultDto>> GetConversations(Guid userId)
    {
        var rooms = _dbContext.ChatRooms
            .Where(x => x.User1Id == userId || x.User2Id == userId)
            .ToList();

        var results = (from room in rooms
        let user = room.User1Id != userId ? room.User1 : room.User2
        select new ConversationWithLastMessageResultDto
        {
            RoomId = room.Id,
            Message = room.Messages.Last().Content,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePicture,
            Date = room.Messages.Last().Date
        }).ToList();

        return Task.FromResult(results);
    }

    public async Task<List<MessageResultDto>> GetMessagesForConversation(Guid roomId, Guid userId)
    {
        var room = await _dbContext.ChatRooms
            .FirstOrDefaultAsync(x => x.Id == roomId);

        if (room is null)
        {
            throw new BadRequestException("invalid_id", "Room does not exists");
        }
        
        if (!(room.User1Id == userId || room.User2Id == userId))
        {
            throw new ForbiddenException("invalid_user", "You are not one of the member from this room");
        }

        var messages = _dbContext.ChatMessages
            .Where(x => x.RoomId == roomId)
            .OrderByDescending(x => x.Date)
            .Take(20)
            .ToList();

        foreach (var message in messages.Where(message => message.UserId != userId && !message.IsRead))
        {
            message.IsRead = true;
            _dbContext.ChatMessages.Update(message);
            await _dbContext.SaveChangesAsync();
        }
        
        return messages
            .Select(message => new MessageResultDto
            {
                Content = message.Content,
                Date = message.Date,
                User = new ChatUserDto
                {
                    FirstName = message.User.FirstName,
                    LastName = message.User.LastName,
                    ProfilePicture = message.User.ProfilePicture
                }
            })
            .ToList();
    }
}