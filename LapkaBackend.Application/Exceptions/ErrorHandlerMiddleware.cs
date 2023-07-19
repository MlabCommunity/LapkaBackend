using Microsoft.AspNetCore.Http;
using System.Text.Json;
using LapkaBackend.Domain.Records;


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
            catch (Exception error)
            {
                var errors = new List<Error>();

                switch (error)
                {
                    case BadRequestException:
                        context.Response.StatusCode = 400;
                        var badRequestException = (BadRequestException)error;
                        errors.Add(new Error(badRequestException.Code, error.Message));
                        break;
                    case UnauthorizezdException:
                        context.Response.StatusCode = 401;
                        var unauthorizedException = (UnauthorizezdException)error;
                        errors.Add(new Error(unauthorizedException.Code, error.Message));
                        break;
                    case ForbiddenExcpetion:
                        context.Response.StatusCode = 403;
                        var forbiddenException = (ForbiddenExcpetion)error;
                        errors.Add(new Error(forbiddenException.Code, error.Message));
                        break;
                    case NotFoundException:
                        context.Response.StatusCode = 404;
                        var notFoundException = (NotFoundException)error;
                        errors.Add(new Error(notFoundException.Code, error.Message));
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        errors.Add(new Error("error", "There was an error"));
                        break;
                }
                var response = context.Response;
                response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { errors });
                await response.WriteAsync(result);
            }

        }
    }
}
