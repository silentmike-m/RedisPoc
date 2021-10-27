namespace RedisPoC.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using RedisPoC.Infrastructure.Redis;
    using RedisPoC.Infrastructure.Redis.Interfaces;
    using RedisPoC.Infrastructure.Redis.Services;

    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();

            services.AddHealthChecks()
                .AddRedis(redisConnectionString: $"{redisOptions.Server},password={redisOptions.Password}", name: "Redis", failureStatus: HealthStatus.Degraded);

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{redisOptions.Server},password={redisOptions.Password}";
                options.InstanceName = redisOptions.InstanceName;
            });

            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
    }
}
