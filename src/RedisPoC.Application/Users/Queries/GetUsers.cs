namespace RedisPoC.Application.Users.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using MediatR;
    using RedisPoC.Application.Users.ViewModels;

    public sealed record GetUsers : IRequest<IReadOnlyList<User>>
    {
        [JsonPropertyName("system_id")] public Guid SystemId { get; init; } = default;
    }
}
