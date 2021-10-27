namespace RedisPoC.Infrastructure.Extensions
{
    using System;
    using System.Text.Json;

    internal static class JsonExtension
    {
        internal static string ToJson<T>(this T source)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(source, options);
        }

        internal static T To<T>(this string source)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.Deserialize<T>(source, options)
                   ?? throw new NullReferenceException();
        }
    }
}
