using API.Infrastructure.Persistence.DbContexts;
using API.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class PersistenceExtension
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("API"))
        );

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

