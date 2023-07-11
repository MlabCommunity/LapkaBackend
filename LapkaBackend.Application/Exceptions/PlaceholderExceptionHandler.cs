using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Exceptions;

public class PlaceholderExceptionHandler
{
    private readonly RequestDelegate _next;
    
    public PlaceholderExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (PlaceholderException e) // In future use switch to determine what kind of status code should be thrown
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = e.Message });
            await context.Response.WriteAsync(result);
        }
    }
}