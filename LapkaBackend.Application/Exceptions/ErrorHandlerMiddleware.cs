using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace LapkaBackend.Application.Exceptions
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AuthException error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = error.StatusCode;

                var result = JsonSerializer.Serialize(new { message = error?.Message });

                await response.WriteAsync(result);
            }

        }
    }
}
