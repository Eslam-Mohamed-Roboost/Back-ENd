namespace API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                if (allowedOrigins == null || allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
                {
                    // For development: Allow all origins without credentials
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    // For production: Specific origins with credentials
                    builder.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static void EnableCors(this WebApplication app)
    {
        app.UseCors();
    }
}

