namespace RedisPoC.WebApi.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RedisPoC.WebApi.Models;

    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ApiControllerBase
    {
        [HttpPost(Name = "Ping")]
        public async Task<BaseResponse<string>> Ping()
        {
            var response = BaseResponse<string>.Success("Pong");
            return await Task.FromResult(response);
        }
    }
}
