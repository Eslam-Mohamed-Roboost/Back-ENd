using System.Text.Json;
using System.Text.Json.Serialization;
using API.Application.Services;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                // Configure enum handling to accept both string and integer values, case-insensitive
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
            });
            
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
            // Configure enum handling for minimal API endpoints (MapPost, MapGet, etc.)
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
        });

        services.AddOpenApi();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        // Register Badge and Hours Services
        services.AddScoped<IBadgeAwardService, BadgeAwardService>();
        services.AddScoped<IHoursTrackingService, HoursTrackingService>();

        return services;
    }
}

