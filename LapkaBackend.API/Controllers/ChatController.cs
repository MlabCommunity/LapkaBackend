using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Interfaces;
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
    
    /// <summary>
    /// Wysyłanie wiadomości do użytkownika, tworzy również pokój jeżeli takowy nie istnieje.
    /// </summary>
    [HttpPost]
    [Authorize (Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Send([Required]string message, [Required] string receiverId)
    {
        await _chatService.SendMessage(message, 
                            new Guid(HttpContext.User.FindFirstValue("userId")!), 
                            new Guid(receiverId));
        
        return NoContent();
    }
    
    /// <summary>
    /// Pobieranie wiadomości dla konkretnych konwersacji, użytkownika aktualnie zalogowanego.
    /// </summary>
    [HttpGet("messages")]
    [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    [ProducesResponseType(typeof(MessageResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetMessagesForConversation([Required] string roomId)
    {
        return Ok(await _chatService
            .GetMessagesForConversation(new Guid(roomId),
                new Guid(HttpContext.User.FindFirstValue("userId")!)));
    }
    
    /// <summary>
    /// Pobranie wszystkich konwersacji użytkownika zalogowanego, wraz z ostanią wiadomośćią do każdej konwersacji
    /// </summary>
    [HttpGet("conversations")]
    [Authorize(Roles = "User,Worker,Admin,SuperAdmin,Shelter")]
    [ProducesResponseType(typeof(ConversationWithLastMessageResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetConversations()
    {
        return Ok(await _chatService
            .GetConversations(new Guid(HttpContext.User.FindFirstValue("userId")!)));
    }
}
