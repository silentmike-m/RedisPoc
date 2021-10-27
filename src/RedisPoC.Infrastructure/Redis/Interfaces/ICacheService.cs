namespace RedisPoC.Infrastructure.Redis.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using RedisPoC.Infrastructure.Redis.Models;

    internal interface ICacheService
    {
        Task<TResponse?> Get<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken = default);
        Task Set<TResponse>(CacheKey<TResponse> key, TResponse value, CancellationToken cancellationToken = default);
    }
}
