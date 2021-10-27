namespace RedisPoC.Application.Common
{
    using System;

    public interface IAuthId
    {
        Guid UserId { get; set; }
    }
}
