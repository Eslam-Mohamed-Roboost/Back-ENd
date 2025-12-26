using API.Infrastructure.Persistence.DbContexts;
using API.Infrastructure.Persistence.Repositories;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Extensions;

public static class PersistenceExtension
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("API"));
            
            // Enable SQL logging with sensitive data (for development)
            if (env.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                
                // Use custom SQL logger with colors
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddProvider(new SqlLoggerProvider());
                    builder.SetMinimumLevel(LogLevel.Debug); // Log all levels including errors
                });
                
                options.UseLoggerFactory(loggerFactory);
                
                // Enable detailed error logging
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed: {ex.Message}");
        }
    }
}

