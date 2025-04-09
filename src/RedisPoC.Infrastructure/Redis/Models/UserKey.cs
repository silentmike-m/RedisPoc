namespace RedisPoC.Infrastructure.Redis.Models
{
    using System;
    using RedisPoC.Application.Users.ViewModels;

    internal sealed record UserKey : CacheKey<User>
    {
        public Guid SystemId { get; init; } = Guid.Empty;
        public Guid UserId { get; init; } = Guid.Empty;
    }
}
