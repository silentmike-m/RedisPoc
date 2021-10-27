#nullable enable
namespace RedisPoC.WebApi.Models
{
    using System.Text.Json.Serialization;

    public sealed class BaseResponse<T>
    {
        [JsonPropertyName("code")] public string Code { get; init; } = "OK";
        [JsonPropertyName("error")] public string? Error { get; init; } = default;
        [JsonPropertyName("response")] public T? Response { get; init; } = default;

        public static BaseResponse<object> Failed(string code, string error)
            => new()
            {
                Code = code,
                Error = error,
            };

        public static BaseResponse<T> Failed(string code, string error, T response)
            => new()
            {
                Code = code,
                Error = error,
                Response = response,
            };

        public static BaseResponse<T> Success(T response)
            => new()
            {
                Response = response,
            };
    }
}
