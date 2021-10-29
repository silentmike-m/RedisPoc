namespace RedisPoC.Infrastructure.Redis.Services
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Infrastructure.Extensions;
    using RedisPoC.Infrastructure.Redis.Interfaces;
    using RedisPoC.Infrastructure.Redis.Models;

    internal sealed class CacheService : ICacheService
    {
        private const int DEFAULT_KEY_SLIDING_EXPIRATION_IN_MINUTES = 1;

        private readonly IDistributedCache cache;
        private readonly ILogger<CacheService> logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse?> Get<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken = default)
        {
            try
            {
                var keyString = key.ToString();
                var bytes = await this.cache.GetAsync(keyString, cancellationToken);

                if (bytes is null)
                {
                    return default;
                }

                this.logger.LogInformation("Got value from cache");

                var json = Encoding.UTF8.GetString(bytes);
                var value = json.To<TResponse>();

                return value;
            }
            catch (Exception exception)
            {
                this.logger.LogWarning(exception, "Error on get cache key value");
                return default;
            }
        }

        public async Task Set<TResponse>(CacheKey<TResponse> key, TResponse value, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(DEFAULT_KEY_SLIDING_EXPIRATION_IN_MINUTES));
            var json = value.ToJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            var keyString = key.ToString();

            try
            {
                await this.cache.SetAsync(keyString, bytes, options, cancellationToken);
            }
            catch (Exception exception)
            {
                this.logger.LogWarning(exception, "Error on set cache key");
            }
        }
    }
}
