using System.Text.Json;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Records;
using ILogger = Serilog.ILogger;

namespace LapkaBackend.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger, IWebHostEnvironment webHostEnvironment)
    {
        _next = next;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
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
                case BadRequestException badRequestException:
                    context.Response.StatusCode = 400;
                    errors.Add(new Error(badRequestException.Code, badRequestException.Message));
                    break;
                case UnauthorizedException unauthorizedException:
                    context.Response.StatusCode = 401;
                    errors.Add(new Error(unauthorizedException.Code, unauthorizedException.Message));
                    break;
                case ForbiddenException forbiddenException:
                    context.Response.StatusCode = 403;
                    errors.Add(new Error(forbiddenException.Code, forbiddenException.Message));
                    break;
                case NotFoundException notFoundException:
                    context.Response.StatusCode = 404;
                    errors.Add(new Error(notFoundException.Code, notFoundException.Message));
                    break;
                default:
                    context.Response.StatusCode = 500;
                    if (_webHostEnvironment.IsProduction())
                    {
                        _logger.Error(error, "server_error");
                    }
                    errors.Add(new Error("error", "Something went wrong"));
                    break;
            }
            var response = context.Response;
            response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { errors });
            await response.WriteAsync(result);
        }

    }
}