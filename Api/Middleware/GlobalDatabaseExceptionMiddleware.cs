using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Models;
using MySqlConnector;

namespace Api.Middleware;

/// <summary>
/// Catches database-related exceptions and returns a consistent JSON error envelope
/// instead of an HTML developer exception page or raw stack trace.
/// </summary>
public class GlobalDatabaseExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalDatabaseExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalDatabaseExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalDatabaseExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            var unwrapped = Unwrap(ex);
            if (unwrapped is MySqlException mysqlEx)
            {
                await WriteErrorAsync(context, MapMySqlException(mysqlEx));
                return;
            }

            if (unwrapped is System.Data.Common.DbException dbEx)
            {
                await WriteErrorAsync(context, MapGenericDbException(dbEx));
                return;
            }

            _logger.LogError(unwrapped, "Unhandled exception.");
            await WriteErrorAsync(context, (
                StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment()
                    ? unwrapped.Message
                    : "An unexpected error occurred."));
        }
    }

    private static Exception Unwrap(Exception ex)
    {
        return ex is AggregateException agg && agg.InnerException is not null
            ? Unwrap(agg.InnerException)
            : ex.InnerException is not null && ex is not MySqlException && ex is not System.Data.Common.DbException
                ? Unwrap(ex.InnerException)
                : ex;
    }

    private (int statusCode, string message) MapMySqlException(MySqlException ex)
    {
        var code = ex.Number != 0 ? ex.Number : (int)ex.ErrorCode;

        return code switch
        {
            1062 => (
                StatusCodes.Status409Conflict,
                FormatDuplicateKeyMessage(ex)),

            1451 => (
                StatusCodes.Status409Conflict,
                "This record is linked to other data and cannot be removed."),

            1452 => (
                StatusCodes.Status400BadRequest,
                "Referenced record does not exist."),

            1048 => (
                StatusCodes.Status400BadRequest,
                "A required field was missing."),

            _ => (
                StatusCodes.Status400BadRequest,
                _environment.IsDevelopment()
                    ? ex.Message
                    : "The request could not be completed due to a database error.")
        };
    }

    private (int statusCode, string message) MapGenericDbException(System.Data.Common.DbException ex)
    {
        return (
            StatusCodes.Status400BadRequest,
            _environment.IsDevelopment() ? ex.Message : "A database error occurred.");
    }

    private static string FormatDuplicateKeyMessage(MySqlException ex)
    {
        var m = ex.Message;
        if (string.IsNullOrEmpty(m))
            return "A value that must be unique already exists.";

        // e.g. Duplicate entry 'deep' for key 'users.UserName'
        if (m.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase))
        {
            if (m.Contains("UserName", StringComparison.OrdinalIgnoreCase) ||
                m.Contains("username", StringComparison.OrdinalIgnoreCase))
                return "This username is already registered.";
            return "A record with this value already exists.";
        }

        return m;
    }

    private static async Task WriteErrorAsync(HttpContext context, (int statusCode, string message) result)
    {
        context.Response.StatusCode = result.statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var body = ApiResponse.Fail(result.message, result.statusCode);
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
