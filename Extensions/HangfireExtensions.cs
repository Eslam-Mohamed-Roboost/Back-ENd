using Hangfire;
using Hangfire.PostgreSql;
using API.Infrastructure.Hangfire;

namespace API.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("Hangfire:Enabled");
        
        if (!enabled)
            return services;

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(options =>
                  {
                      options.UseNpgsqlConnection(connectionString);
                  });
        });

        // Add Hangfire server
        var workerCount = configuration.GetValue<int>("Hangfire:WorkerCount", 5);
        var queues = configuration.GetSection("Hangfire:Queues").Get<string[]>() ?? new[] { "default" };
        
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = workerCount;
            options.Queues = queues;
            options.ServerName = $"{Environment.MachineName}:{Guid.NewGuid()}";
        });

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboardMiddleware(this IApplicationBuilder app, IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("Hangfire:Enabled");
        var dashboardEnabled = configuration.GetValue<bool>("Hangfire:DashboardEnabled");
        
        if (enabled && dashboardEnabled)
        {
            var dashboardPath = configuration["Hangfire:DashboardPath"] ?? "/hangfire";
            var username = configuration["Hangfire:Authorization:Username"];
            var password = configuration["Hangfire:Authorization:Password"];
            
            app.UseHangfireDashboard(dashboardPath, new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter(username, password)
                },
                DashboardTitle = "Sira API - Background Jobs",
                StatsPollingInterval = 5000
            });
        }

        return app;
    }
}

