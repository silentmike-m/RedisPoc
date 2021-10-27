namespace RedisPoC.Infrastructure.Redis
{
    internal sealed class RedisOptions
    {
        public static readonly string SectionName = "Redis";
        public string InstanceName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;

    }
}
