namespace RedisPoC.Infrastructure.Redis.Models
{
    using System.Text.Json;

    // ReSharper disable once UnusedTypeParameter
#pragma warning disable S2326 // Unused type parameters should be removed
    internal abstract record CacheKey<TResponse>
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public string Type { get; }

        protected CacheKey()
        {
            this.Type = this.GetType().ToString();
        }

        public override string ToString()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
