namespace RedisPoC.Infrastructure.HealthCheck.Models
{
    using System.Collections.Generic;

    public sealed record MainHealthCheck
    {
        public IReadOnlyList<ComponentHealthCheck> HealthChecks { get; init; } = new List<ComponentHealthCheck>().AsReadOnly();
        public int HealthCheckDurationInMilliseconds { get; init; } = default;
        public string Status { get; init; } = string.Empty;
    }
}
