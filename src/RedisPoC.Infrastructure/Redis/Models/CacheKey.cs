namespace RedisPoC.Infrastructure.Redis.Models
{
    using System;
    using System.Text.Json;

    // ReSharper disable once UnusedTypeParameter
#pragma warning disable S2326 // Unused type parameters should be removed
    internal abstract class CacheKey<TResponse>
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        // ReSharper disable once MemberCanBePrivate.Global

        private Type KeyType { get; }
        public string Type { get; }

        protected CacheKey()
        {
            this.KeyType = this.GetType();
            this.Type = this.KeyType.ToString();
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
            };

            return JsonSerializer.Serialize(this, this.KeyType, options);
        }
    }
}
