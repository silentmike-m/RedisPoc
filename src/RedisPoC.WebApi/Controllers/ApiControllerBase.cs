namespace RedisPoC.WebApi.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender mediator;
        protected ISender Mediator => this.mediator ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
