namespace RedisPoC.Infrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Infrastructure.Redis;
    using RedisPoC.Infrastructure.Redis.Interfaces;
    using RedisPoC.Infrastructure.Redis.Services;
    using StackExchange.Redis;

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

        public static void UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("DependencyInjection");

            try
            {
                var redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
                var connection = ConnectionMultiplexer.Connect($"{redisOptions.Server},password={redisOptions.Password}");
                connection.Subscribe(logger);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, exception.Message);
            }
        }
    }
}
