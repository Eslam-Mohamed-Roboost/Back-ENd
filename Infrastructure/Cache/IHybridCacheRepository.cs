namespace API.Infrastructure.Cache;

public interface IHybridCacheRepository
{
    Task SetAsync<T>(string cacheKey, T value, TimeSpan duration) where T : class, new();
    Task<T> GetAsync<T>(string cacheKey) where T : class, new();
    Task RemoveAsync<T>(string cacheKey) where T : class, new();
}

