namespace RedisPoC.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RedisPoC.Application.Users.Queries;
    using RedisPoC.Application.Users.ViewModels;
    using RedisPoC.WebApi.Models;

    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]

    public sealed class UserController : ApiControllerBase
    {
        [HttpPost(Name = "GetUser")]
        public async Task<BaseResponse<User>> GetUser(GetUser request)
        {
            var result = await this.Mediator.Send(request);
            return BaseResponse<User>.Success(result);
        }

        [HttpPost(Name = "GetUsers")]
        public async Task<BaseResponse<IReadOnlyList<User>>> GetUsers(GetUsers request)
        {
            var result = await this.Mediator.Send(request);
            return BaseResponse<IReadOnlyList<User>>.Success(result);
        }
    }
}
