namespace RedisPoC.Application.Users.ViewModels
{
    using System;
    using System.Text.Json.Serialization;

    public sealed record User
    {
        [JsonPropertyName("id")] public Guid Id { get; init; } = default;
        [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    }
}
