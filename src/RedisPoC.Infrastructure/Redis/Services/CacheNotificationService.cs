namespace RedisPoC.Infrastructure.Redis.Services
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Infrastructure.Redis.Models;
    using StackExchange.Redis;

    internal static class CacheNotificationService
    {
        private const string EXPIRED_KEYS_CHANNEL = "__keyspace@0__:*";
        private const string KEY_PATTERN = "RedisPoC.Infrastructure.Redis.Models.[a-zA-Z]+";

        public static void Subscribe(this ConnectionMultiplexer connection, ILogger logger)
        {
            var subscriber = connection.GetSubscriber();

            subscriber.Subscribe(EXPIRED_KEYS_CHANNEL, (channel, notificationType) =>
            {
                var key = GetKey(channel);

                if (key == typeof(UsersKey).FullName)
                {
                    ProceedUsersKey(notificationType);
                }
            });
        }

        private static string GetKey(string channel)
        {
            var regex = new Regex(KEY_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            var match = regex.Match(channel);

            return match.Success ? match.Value : channel;
        }

        private static void ProceedUsersKey(RedisValue notificationType)
        {
            switch (notificationType)
            {
                case "expired":
                    Console.WriteLine("Users key expired");
                    break;
            }
        }
    }
}
