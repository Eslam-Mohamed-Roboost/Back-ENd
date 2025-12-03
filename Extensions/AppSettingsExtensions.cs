namespace API.Extensions;

public static class AppSettingsExtensions
{
    public static void ReadAppSettings(this WebApplicationBuilder builder)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");

        builder.Configuration
            .SetBasePath(configPath)
            .AddJsonFile("Base/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Base/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Database/appsettings.database.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Jwt/appsettings.jwt.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Serilog/appsettings.serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Redis/appsettings.redis.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"RateLimiting/appsettings.rate-limiting.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"TimeOut/appsettings.timeout.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    }
}

