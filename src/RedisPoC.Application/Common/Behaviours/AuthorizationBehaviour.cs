namespace RedisPoC.Application.Common.Behaviours
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    internal sealed class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentRequestService currentUserService;

        public AuthorizationBehaviour(ICurrentRequestService currentUserService)
        {
            this.currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IAuthId authId)
            {
                var userId = this.currentUserService.CurrentUserId;

                if (authId.UserId == Guid.Empty)
                {
                    authId.UserId = userId;
                }
            }

            return await next();
        }
    }
}
