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
        // Check if response has already started - if so, we can't modify headers or status code
        if (context.Response.HasStarted)
        {
            // Response already started, can't modify headers or write error response
            // The exception will be logged but we can't send a proper error response
            return Task.CompletedTask;
        }

        try
        {
            // Try to clear any existing response content
            context.Response.Clear();
        }
        catch
        {
            // If clear fails (response already committed), we can't proceed
            return Task.CompletedTask;
        }

        context.Response.ContentType = "application/json";
        
        // Determine status code and message based on exception type
        var (statusCode, message, details) = exception switch
        {
            JsonException jsonEx when jsonEx.Message.Contains("could not be converted") && jsonEx.Message.Contains("Enum") =>
                (400, "Invalid enum value provided.", GetEnumErrorMessage(jsonEx.Message) ?? jsonEx.Message),
            JsonException jsonEx =>
                (400, "Invalid JSON format.", jsonEx.Message),
            Microsoft.AspNetCore.Http.BadHttpRequestException badRequest =>
                (400, "Invalid request format.", badRequest.Message),
            _ =>
                (500, "An unexpected error occurred.", exception.Message)
        };

        context.Response.StatusCode = statusCode;

        var errorResponse = new
        {
            Message = message,
            Details = details,
            StatusCode = statusCode
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(jsonResponse);
    }

    private static string? GetEnumErrorMessage(string jsonExceptionMessage)
    {
        // Extract enum type and path from error message
        // Example: "The JSON value could not be converted to API.Domain.Enums.ApplicationRole. Path: $.RoleID"
        if (jsonExceptionMessage.Contains("ApplicationRole"))
        {
            return "Invalid RoleID value. Valid values are: 1 (Admin), 3 (Teacher), 4 (Student), or their string equivalents (Admin, Teacher, Student).";
        }
        return null;
    }
}


