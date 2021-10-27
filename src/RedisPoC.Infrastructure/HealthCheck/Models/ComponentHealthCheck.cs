namespace RedisPoC.Infrastructure.HealthCheck.Models
{
    public sealed record ComponentHealthCheck
    {
        public string Component { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string ErrorMessage { get; init; } = string.Empty;
        public int HealthCheckDurationInMilliseconds { get; init; } = default;
        public string Status { get; init; } = string.Empty;
    }
}
