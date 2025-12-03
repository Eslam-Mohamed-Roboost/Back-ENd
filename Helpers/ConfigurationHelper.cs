namespace API.Helpers;

public static class ConfigurationHelper
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string GetConfigurationValue(string key)
    {
        return _configuration[key];
    }

    public static T GetSection<T>(string key)
    {
        var section = _configuration.GetSection(key);
        return section.Exists() ? section.Get<T>() : default;
    }

    public static string GetConnectionString(string name = "DefaultConnection")
    {
        return _configuration.GetConnectionString(name);
    }

    public static string GetApplicationName()
    {
        return GetConfigurationValue("Serilog:Properties:ApplicationName") ?? "SiraAPI";
    }

    public static bool IsEnvironment(string name)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(name.Trim(), environment.Trim(), StringComparison.CurrentCultureIgnoreCase);
    }
}

