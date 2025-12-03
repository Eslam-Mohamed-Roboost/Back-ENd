using System.Diagnostics;

namespace API.Middlewares;

public class SlowRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SlowRequestLoggingMiddleware> _logger;
    private readonly TimeSpan _slowRequestThreshold;

    public SlowRequestLoggingMiddleware(RequestDelegate next, ILogger<SlowRequestLoggingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        // Read threshold from configuration (default 3 seconds)
        var thresholdSeconds = configuration.GetValue<int>("SlowRequestThreshold", 3);
        _slowRequestThreshold = TimeSpan.FromSeconds(thresholdSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        if (stopwatch.Elapsed > _slowRequestThreshold)
        {
            _logger.LogWarning(
                "⚠️ Slow Request Detected! Path: {Path}, Method: {Method}, Time Taken: {ElapsedMilliseconds}ms",
                context.Request.Path,
                context.Request.Method,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}

