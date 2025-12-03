using System.Diagnostics;

namespace API.Middlewares;

public class RequestTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimeoutMiddleware> _logger;

    public RequestTimeoutMiddleware(RequestDelegate next, ILogger<RequestTimeoutMiddleware> _logger)
    {
        _next = next;
        this._logger = _logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        int timeoutSeconds = 30; // Default timeout

        var timeoutAttr = endpoint?.Metadata.GetMetadata<TimeOutAttribute>();

        if (timeoutAttr != null)
        {
            timeoutSeconds = timeoutAttr.TimeoutSeconds;

            if (timeoutSeconds <= 0)
            {
                _logger.LogInformation("Request to {Path} has NO timeout.", context.Request.Path);
                await _next(context);
                return;
            }

            _logger.LogInformation("Request to {Path} has a custom timeout of {Timeout} seconds.",
                context.Request.Path, timeoutSeconds);
        }

        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, context.RequestAborted);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next.Invoke(context).WaitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning("Request to {Path} timed out after {ElapsedMilliseconds}ms", 
                context.Request.Path, stopwatch.ElapsedMilliseconds);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 408;
                await context.Response.WriteAsync("Request timeout");
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class TimeOutAttribute : Attribute
{
    public int TimeoutSeconds { get; }

    public TimeOutAttribute(int timeoutSeconds = 30)
    {
        TimeoutSeconds = timeoutSeconds;
    }
}

