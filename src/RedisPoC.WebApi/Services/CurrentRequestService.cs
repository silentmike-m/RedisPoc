namespace RedisPoC.WebApi.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using RedisPoC.Application.Common;

    [ExcludeFromCodeCoverage]
    internal sealed class CurrentRequestService : ICurrentRequestService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentRequestService(IHttpContextAccessor httpContextAccessor)
            => (this.httpContextAccessor) = (httpContextAccessor);

        public Guid CurrentUserId
        {
            get
            {
                var userIdClaim = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Guid.Empty;
                }

                return new Guid(userIdClaim);
            }
        }
    }
}
