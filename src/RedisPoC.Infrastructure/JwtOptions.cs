namespace RedisPoC.Infrastructure
{
    public sealed record JwtOptions
    {
        public static readonly string SectionName = "Jwt";
        public string Audience { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string SecurityKey { get; init; } = string.Empty;
    }
}
