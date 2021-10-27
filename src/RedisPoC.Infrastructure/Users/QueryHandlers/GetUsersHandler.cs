namespace RedisPoC.Infrastructure.Users.QueryHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Application.Users.Queries;
    using RedisPoC.Application.Users.ViewModels;
    using RedisPoC.Infrastructure.Redis.Interfaces;
    using RedisPoC.Infrastructure.Redis.Models;

    internal sealed class GetUsersHandler : IRequestHandler<GetUsers, IReadOnlyList<User>>
    {
        private readonly ICacheService cacheService;
        private readonly ILogger<GetUsersHandler> logger;

        public GetUsersHandler(ICacheService cacheService, ILogger<GetUsersHandler> logger)
        {
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<IReadOnlyList<User>> Handle(GetUsers request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Try to get users from system with id {SystemId}", request.SystemId);

            var key = new UsersKey
            {
                SystemId = request.SystemId,
            };

            var users = await this.cacheService.Get(key, cancellationToken);

            if (users is null)
            {
                users = new List<User>
                {
                    new() { Id = Guid.NewGuid(), Name = "user name 1" },
                    new() { Id = Guid.NewGuid(), Name = "user name 2" },
                };

                await this.cacheService.Set(key, users, cancellationToken);
            }

            var result = users.ToList();

            return await Task.FromResult(result);
        }
    }
}
