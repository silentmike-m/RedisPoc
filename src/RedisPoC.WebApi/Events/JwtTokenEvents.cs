namespace RedisPoC.WebApi.Events
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    public sealed class JwtTokenEvents : JwtBearerEvents
    {
        public override Task MessageReceived(MessageReceivedContext context)
        {
            var accessToken = context.Request.Query["access_token"];

            if (string.IsNullOrEmpty(accessToken) && context.Request.Headers.TryGetValue("Authorization", out var value))
            {
                accessToken = value.ToString().Replace("Bearer ", string.Empty);
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }

            return base.MessageReceived(context);
        }
    }
}
