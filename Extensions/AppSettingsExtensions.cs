namespace API.Extensions;

public static class AppSettingsExtensions
{
    public static void ReadAppSettings(this WebApplicationBuilder builder)
    {
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        
        // Disable reloadOnChange to avoid inotify limit issues
        // If you need hot reload, set DOTNET_USE_POLLING_FILE_WATCHER=1 environment variable
        // and enable reloadOnChange: true below
        var reloadOnChange = false;

        builder.Configuration
            .SetBasePath(configPath)
            .AddJsonFile("Base/appsettings.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"Base/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"Database/appsettings.database.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"Jwt/appsettings.jwt.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"Serilog/appsettings.serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"Redis/appsettings.redis.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"RateLimiting/appsettings.rate-limiting.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"TimeOut/appsettings.timeout.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);
    }
}

