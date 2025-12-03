using Microsoft.Extensions.Caching.Hybrid;

namespace API.Infrastructure.Cache;

public class HybridCacheRepository : IHybridCacheRepository
{
    private readonly HybridCache _cache;
    private readonly ILogger<HybridCacheRepository> _logger;

    public HybridCacheRepository(HybridCache cache, ILogger<HybridCacheRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string key) where T : class, new()
    {
        var cachedKey = $":{typeof(T).Name}:{key}";

        try
        {
            return await _cache.GetOrCreateAsync(cachedKey, ct => ValueTask.FromResult<T>(default))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan duration) where T : class, new()
    {
        var cachedKey = $":{typeof(T).Name}:{key}";

        try
        {
            await _cache.SetAsync(cachedKey, value, new HybridCacheEntryOptions
            {
                Expiration = duration,
                LocalCacheExpiration = duration
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching {CacheKey}", cachedKey);
        }
    }

    public async Task RemoveAsync<T>(string key) where T : class, new()
    {
        var cachedKey = $":{typeof(T).Name}:{key}";

        try
        {
            _logger.LogWarning("Removing {CacheKey} from cache", cachedKey);
            await _cache.RemoveAsync(cachedKey).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing {CacheKey}", cachedKey);
        }
    }
}

