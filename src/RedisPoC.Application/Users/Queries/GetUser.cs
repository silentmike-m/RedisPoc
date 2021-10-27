﻿namespace RedisPoC.Application.Users.Queries
{
    using System;
    using System.Text.Json.Serialization;
    using MediatR;
    using RedisPoC.Application.Users.ViewModels;

    public sealed record GetUser : IRequest<User>
    {
        [JsonPropertyName("user_id")] public Guid UserId { get; init; } = default;
        [JsonPropertyName("system_id")] public Guid SystemId { get; init; } = default;
    }
}
