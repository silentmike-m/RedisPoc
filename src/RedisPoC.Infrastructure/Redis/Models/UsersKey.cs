namespace RedisPoC.Infrastructure.Redis.Models
{
    using System;
    using System.Collections.Generic;
    using RedisPoC.Application.Users.ViewModels;

    internal sealed record UsersKey : CacheKey<IList<User>>
    {
        public Guid SystemId { get; init; } = default;
    }
}
