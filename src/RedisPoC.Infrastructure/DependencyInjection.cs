namespace RedisPoC.Infrastructure
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
