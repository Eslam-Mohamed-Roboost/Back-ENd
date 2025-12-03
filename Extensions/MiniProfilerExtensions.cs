using StackExchange.Profiling;

namespace API.Extensions;

public static class MiniProfilerExtensions
{
    public static IServiceCollection ConfigureMiniProfiler(this IServiceCollection services, IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("MiniProfiler:Enabled");
        
        if (!enabled)
            return services;

        services.AddMiniProfiler(options =>
        {
            options.RouteBasePath = configuration["MiniProfiler:RouteBasePath"] ?? "/profiler";
            options.ColorScheme = Enum.Parse<ColorScheme>(configuration["MiniProfiler:ColorScheme"] ?? "Auto");
            
            // SQL Profiling
            options.EnableServerTimingHeader = true;
            
            // Stack trace settings
            options.StackMaxLength = configuration.GetValue<int>("MiniProfiler:StackMaxLength", 100);
            
            // Popup settings
            options.PopupShowTimeWithChildren = configuration.GetValue<bool>("MiniProfiler:PopupShowTimeWithChildren", true);
            options.PopupShowTrivial = configuration.GetValue<bool>("MiniProfiler:PopupShowTrivial", true);
            
            // Ignore paths
            options.ShouldProfile = request =>
            {
                var path = request.Path.Value ?? string.Empty;
                return !path.StartsWith("/hangfire") && 
                       !path.StartsWith("/cap") &&
                       !path.StartsWith("/scalar");
            };
        }).AddEntityFramework();

        return services;
    }

    public static IApplicationBuilder UseMiniProfilerMiddleware(this IApplicationBuilder app, IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("MiniProfiler:Enabled");
        
        if (enabled)
        {
            app.UseMiniProfiler();
        }

        return app;
    }
}

