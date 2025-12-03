using DotNetCore.CAP;
using DotNetCore.CAP.Dashboard.NodeDiscovery;

namespace API.Extensions;

public static class CAPExtensions
{
    public static IServiceCollection ConfigureCAP(this IServiceCollection services, IConfiguration configuration)
    {
        var enabled = configuration.GetValue<bool>("CAP:Enabled");
        
        if (!enabled)
            return services;

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddCap(options =>
        {
            // Use PostgreSQL for message persistence
            options.UsePostgreSql(connectionString);
            
            // Use RabbitMQ for message transport
            var rabbitMQConfig = configuration.GetSection("CAP:RabbitMQ");
            options.UseRabbitMQ(rabbitMQ =>
            {
                rabbitMQ.HostName = rabbitMQConfig["HostName"] ?? "localhost";
                rabbitMQ.Port = rabbitMQConfig.GetValue<int>("Port", 5672);
                rabbitMQ.UserName = rabbitMQConfig["UserName"] ?? "guest";
                rabbitMQ.Password = rabbitMQConfig["Password"] ?? "guest";
                rabbitMQ.VirtualHost = rabbitMQConfig["VirtualHost"] ?? "/";
                rabbitMQ.ExchangeName = rabbitMQConfig["ExchangeName"] ?? "sira.eventbus";
            });
            
            // Dashboard configuration
            var dashboardEnabled = configuration.GetValue<bool>("CAP:DashboardEnabled");
            if (dashboardEnabled)
            {
                options.UseDashboard(dashboard =>
                {
                    dashboard.PathMatch = configuration["CAP:DashboardPath"] ?? "/cap";
                    dashboard.StatsPollingInterval = 5000;
                });
            }
            
            // Retry configuration
            options.FailedRetryCount = configuration.GetValue<int>("CAP:FailedRetryCount", 3);
            options.FailedRetryInterval = configuration.GetValue<int>("CAP:FailedRetryInterval", 60);
            
            // Message versioning
            options.Version = "v1";
            
            // JSON serialization
           // options.UseNewtonsoftJson();
        });

        return services;
    }
}

