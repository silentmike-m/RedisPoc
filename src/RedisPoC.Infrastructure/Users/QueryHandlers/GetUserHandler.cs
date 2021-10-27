namespace RedisPoC.Infrastructure.Users.QueryHandlers
{
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

    internal sealed class GetUserHandler : IRequestHandler<GetUser, User>
    {
        private readonly ICacheService cacheService;
        private readonly ILogger<GetUserHandler> logger;

        public GetUserHandler(ICacheService cacheService, ILogger<GetUserHandler> logger)
        {
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<User> Handle(GetUser request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Try to get user with id {UserId} from system with id {SystemId}", request.UserId, request.SystemId);

            var userKey = new UserKey
            {
                SystemId = request.SystemId,
                UserId = request.UserId,
            };

            this.logger.LogInformation(userKey.ToString());

            var user = await this.cacheService.Get(userKey, cancellationToken);

            if (user is null)
            {
                var key = new UsersKey
                {
                    SystemId = request.SystemId,
                };

                this.logger.LogInformation(key.ToString());

                var users = await this.cacheService.Get(key, cancellationToken);

                user = users?.SingleOrDefault(i => i.Id == request.UserId);
            }

            if (user is null)
            {
                throw new KeyNotFoundException("User not found");
            }

            await this.cacheService.Set(userKey, user, cancellationToken);

            return await Task.FromResult(user);
        }
    }
}
