using System.Text.Json;

namespace API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            Message = "An unexpected error occurred.",
            Details = exception.Message,
            StatusCode = 500
        };

        context.Response.StatusCode = 500;
        var jsonResponse = JsonSerializer.Serialize(errorResponse);

        return context.Response.WriteAsync(jsonResponse);
    }
}

