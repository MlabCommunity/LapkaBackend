using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Interfaces;

public interface IChatService
{
    Task SendMessage(string message, Guid sender, Guid receiver);
    Task<List<ConversationWithLastMessageResultDto>> GetConversations(Guid userId);
    Task<List<MessageResultDto>> GetMessagesForConversation(Guid roomId, Guid userId);
    Task LeaveConversation(Guid roomId);

}