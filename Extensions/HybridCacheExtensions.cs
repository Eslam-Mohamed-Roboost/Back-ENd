using API.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Hybrid;
using StackExchange.Redis;

namespace API.Extensions;

public static class HybridCacheExtensions
{
    public static void ConfigureHybridCache(this WebApplicationBuilder builder)
    {
        var redisConnectionString = builder.Configuration.GetValue<string>("RedisSettings:ConnectionString");
        var redisEnabled = builder.Configuration.GetValue<bool>("RedisSettings:Enabled");

        if (redisEnabled && !string.IsNullOrEmpty(redisConnectionString))
        {
            builder.Services.AddStackExchangeRedisCache(action =>
            {
                var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
                configurationOptions.ConnectTimeout = 500;
                configurationOptions.ConnectRetry = 2;
                configurationOptions.AsyncTimeout = 500;
                configurationOptions.SyncTimeout = 500;

                action.ConfigurationOptions = configurationOptions;
            });
        }

        builder.Services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 512 * 1024;
            options.MaximumKeyLength = 512;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(15),
                LocalCacheExpiration = TimeSpan.FromMinutes(15)
            };
        });

        builder.Services.AddScoped<IHybridCacheRepository, HybridCacheRepository>();
    }
}

