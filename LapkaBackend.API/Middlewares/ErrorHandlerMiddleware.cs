using System.Text.Json;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Records;
using ILogger = Serilog.ILogger;

namespace LapkaBackend.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Error exception;
            switch (error)
            {
                case BadRequestException badRequestException:
                    context.Response.StatusCode = 400;
                    exception = new Error(badRequestException.Code, badRequestException.Message);
                    break;
                
                case UnauthorizedException unauthorizedException:
                    context.Response.StatusCode = 401;
                    exception = new Error(unauthorizedException.Code, unauthorizedException.Message);
                    break;
                
                case ForbiddenException forbiddenException:
                    context.Response.StatusCode = 403;
                    exception = new Error(forbiddenException.Code, forbiddenException.Message);
                    break;
                
                case NotFoundException notFoundException:
                    context.Response.StatusCode = 404;
                    exception = new Error(notFoundException.Code, notFoundException.Message);
                    break;
                
                default:
                    context.Response.StatusCode = 500;
                    if (environment == Environments.Development)
                    {
                        exception = new LocalError("error", error.Message.Replace(Environment.NewLine, " "), 
                            error.StackTrace!.Split(Environment.NewLine).ToList());
                    }
                    
                    else
                    {
                        _logger.Error(error, "server_error");
                        exception = new Error("error", "Something went wrong");
                    }
                    
                    break;
            }
            
            var response = context.Response;
            response.ContentType = "application/json";
            var result = environment == Environments.Development && context.Response.StatusCode == 500 ?
                JsonSerializer.Serialize((LocalError)exception) : JsonSerializer.Serialize(exception);
            await response.WriteAsync(result);
        }

    }
}