namespace RedisPoC.Infrastructure.Redis.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using RedisPoC.Infrastructure.Extensions;
    using RedisPoC.Infrastructure.Redis.Models;
    using StackExchange.Redis;

    [ExcludeFromCodeCoverage]
    internal static class CacheNotificationService
    {
        private const string EXPIRED_KEYS_CHANNEL = "__keyevent@0__:expired";
        private const string KEY_PATTERN = "RedisPoC.Infrastructure.Redis.Models.[a-zA-Z]+";
        private const string KEY_VALUE_PATTERN = "{\".+\"}";

        public static void Subscribe(ILogger logger, RedisOptions options)
        {
            var connection = ConnectionMultiplexer.Connect($"{options.Server},password={options.Password}");
            var subscriber = connection.GetSubscriber();


            subscriber.Subscribe(EXPIRED_KEYS_CHANNEL, (_, keyString) =>
            {
                logger.LogInformation("Try to process expired cache key: {CacheKey}", keyString);

                var key = GetKey(keyString);

                if (key?.keyName == typeof(UserKey).FullName)
                {
                    UserKeyExpired(key!.Value.ketValue, logger);
                }
                else if (key?.keyName == typeof(UsersKey).FullName)
                {
                    UsersKeyExpired(key!.Value.ketValue, logger);
                }
                else
                {
                    logger.LogInformation("Unsupported cache key");
                }
            });
        }

        private static (string keyName, string ketValue)? GetKey(string keyString)
        {
            var nameRegex = new Regex(KEY_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            var nameMatch = nameRegex.Match(keyString);

            if (!nameMatch.Success)
            {
                return null;
            }

            var valueRegex = new Regex(KEY_VALUE_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            var valueMatch = valueRegex.Match(keyString);

            if (!valueMatch.Success)
            {
                return null;
            }

            return (nameMatch.Value, valueMatch.Value);
        }

        private static void UserKeyExpired(string keyValue, ILogger logger)
        {
            var key = keyValue.To<UserKey>();

            logger.LogInformation("Proceed expired user key with system id {SystemId}", key.SystemId);
        }

        private static void UsersKeyExpired(string keyValue, ILogger logger)
        {
            var key = keyValue.To<UsersKey>();

            logger.LogInformation("Proceed expired users key with system id {SystemId}", key.SystemId);
        }
    }
}
