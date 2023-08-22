using System.Security.Claims;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    [Authorize (Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    public async Task<ActionResult> Send(string message, string receiverId)
    {
        await _chatService.SendMessage(message, 
                            new Guid(HttpContext.User.FindFirstValue("userId")!), 
                            new Guid(receiverId));
        
        return NoContent();
    }

    [HttpGet("messages")]
    [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    public async Task<ActionResult> GetMessagesForConversation(string roomId)
    {
        return Ok(await _chatService
            .GetMessagesForConversation(new Guid(roomId),
                new Guid(HttpContext.User.FindFirstValue("userId")!)));
    }
    
    [HttpGet("conversations")]
    [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    public async Task<ActionResult> GetConversations()
    {
        return Ok(await _chatService
            .GetConversations(new Guid(HttpContext.User.FindFirstValue("userId")!)));
    }
}
