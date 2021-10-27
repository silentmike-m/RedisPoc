namespace RedisPoC.Infrastructure.HealthCheck.Extensions
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using RedisPoC.Infrastructure.HealthCheck.Models;

    public static class HealthReportExtensions
    {
        public static MainHealthCheck GetHealthCheck(this HealthReport report)
        {
            var componentHealthChecks = new List<ComponentHealthCheck>();

            foreach (var (componentName, entry) in report.Entries)
            {
                var componentHealthCheck = new ComponentHealthCheck
                {
                    Component = componentName,
                    Description = entry.Description ?? string.Empty,
                    ErrorMessage = entry.Exception?.Message ?? string.Empty,
                    HealthCheckDurationInMilliseconds = entry.Duration.Milliseconds,
                    Status = entry.Status.ToString(),
                };
                componentHealthChecks.Add(componentHealthCheck);
            }

            var healthCheck = new MainHealthCheck
            {
                HealthChecks = componentHealthChecks,
                HealthCheckDurationInMilliseconds = report.TotalDuration.Milliseconds,
                Status = report.Status.ToString(),
            };

            return healthCheck;
        }
    }
}
