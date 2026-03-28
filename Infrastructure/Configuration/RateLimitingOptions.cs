namespace Infrastructure.Configuration;

/// <summary>ASP.NET Core rate limiting (per client IP). Tuned via appsettings.</summary>
public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    /// <summary>Max requests per IP per global window (default policy for all endpoints).</summary>
    public int GlobalPermitLimit { get; set; } = 100;

    /// <summary>Length of the sliding window for the global limiter.</summary>
    public int GlobalWindowSeconds { get; set; } = 60;

    /// <summary>Segments per window (higher = smoother; typical 4–6).</summary>
    public int GlobalSegmentsPerWindow { get; set; } = 4;

    /// <summary>Stricter cap for auth endpoints (login/refresh) per IP.</summary>
    public int AuthPermitLimit { get; set; } = 10;

    public int AuthWindowSeconds { get; set; } = 60;

    /// <summary>When true, honor X-Forwarded-For (use only behind a trusted reverse proxy).</summary>
    public bool UseForwardedHeaders { get; set; }
}
