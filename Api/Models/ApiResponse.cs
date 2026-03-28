using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Api.Models;

/// <summary>
/// Single envelope for every API response.
/// Success: <see cref="Success"/> true, <see cref="ErrorMsg"/> empty, <see cref="Data"/> holds the payload.
/// Failure: <see cref="Success"/> false, <see cref="Data"/> null (no payload), <see cref="ErrorMsg"/> explains the error.
/// </summary>
public sealed class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("error")]
    public bool Error { get; init; }

    [JsonPropertyName("error_msg")]
    public string ErrorMsg { get; init; } = string.Empty;

    [JsonPropertyName("status_code")]
    public int StatusCode { get; init; }

    [JsonPropertyName("data")]
    public object? Data { get; init; }

    /// <summary>Successful response; <paramref name="errorMsg"/> is always cleared.</summary>
    public static ApiResponse Ok(object? data = null, int statusCode = StatusCodes.Status200OK) => new()
    {
        Success = true,
        Error = false,
        ErrorMsg = string.Empty,
        StatusCode = statusCode,
        Data = data
    };

    /// <summary>Failed response; <paramref name="data"/> is always cleared.</summary>
    public static ApiResponse Fail(string errorMsg, int statusCode = StatusCodes.Status400BadRequest) => new()
    {
        Success = false,
        Error = true,
        ErrorMsg = string.IsNullOrEmpty(errorMsg) ? "Request failed." : errorMsg,
        StatusCode = statusCode,
        Data = null
    };
}
