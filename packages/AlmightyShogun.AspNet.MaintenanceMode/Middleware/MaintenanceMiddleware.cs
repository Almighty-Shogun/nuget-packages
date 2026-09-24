using System.Net;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Enforces maintenance windows for incoming HTTP requests.
/// </summary>
///
/// <param name="next">The next request delegate.</param>
/// <param name="maintenanceService">Provides the current maintenance state.</param>
/// <param name="maintenanceOptions">The configured maintenance settings.</param>
/// <param name="responseWriter">Writes error responses for blocked requests.   </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class MaintenanceMiddleware(
    RequestDelegate next,
    MaintenanceService maintenanceService,
    IOptions<MaintenanceSettings> maintenanceOptions,
    IHttpErrorResponseWriter responseWriter
)
{
    /// <summary>
    /// The normalized maintenance endpoint path.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly PathString _maintenancePath = MaintenancePath.Normalize(maintenanceOptions.Value.MaintenancePath, "/maintenance");

    /// <summary>
    /// Handles maintenance requests and enforces the current window.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task that completes when the request has been handled.</returns>
    ///
    /// <remarks>
    /// The maintenance endpoint returns the window as JSON with status 503 when active,
    /// or status 404 otherwise. Other requests pass through unless the active window
    /// blocks them.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task InvokeAsync(HttpContext context)
    {
        PersistedMaintenanceState state = await maintenanceService.GetPersistedAsync();

        bool active = IsWindowActive(state);

        if (context.Request.Path.Equals(_maintenancePath, StringComparison.OrdinalIgnoreCase))
        {
            if (!active)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                return;
            }

            await WriteDetailsAsync(context, state);

            return;
        }

        if (!active || ShouldPassThrough(context, state))
        {
            await next(context);

            return;
        }

        SetRetryAfter(context, state);

        if (state.RedirectBlockedRequests && AcceptsHtml(context.Request))
        {
            context.Response.Redirect(context.Request.PathBase.Add(_maintenancePath));

            return;
        }

        await responseWriter.WriteAsync(
            context,
            StatusCodes.Status503ServiceUnavailable,
            "service_unavailable",
            state.Message,
            context.RequestAborted);
    }

    /// <summary>
    /// Writes the active window to the maintenance endpoint.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="state">The current maintenance window.</param>
    ///
    /// <returns>A task that completes when the response has been written.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static async Task WriteDetailsAsync(HttpContext context, PersistedMaintenanceState state)
    {
        SetRetryAfter(context, state);

        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        await context.Response.WriteAsJsonAsync(
            new MaintenanceResponse
            {
                Message = state.Message,
                StartsAt = state.StartsAt,
                EndsAt = state.EndsAt,
                EnabledAt = state.EnabledAt
            },
            context.RequestAborted);
    }

    /// <summary>
    /// Sets Retry-After to the remaining seconds when the estimated end is in the future.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="state">The current maintenance window.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void SetRetryAfter(HttpContext context, PersistedMaintenanceState state)
    {
        if (state.EndsAt is not { } endsAt || context.Response.HasStarted) return;

        double seconds = Math.Ceiling((endsAt - DateTimeOffset.UtcNow).TotalSeconds);

        if (seconds > 0)
            context.Response.Headers.RetryAfter = seconds.ToString("F0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Checks whether an enabled maintenance window has started.
    /// </summary>
    ///
    /// <param name="state">The current maintenance window.</param>
    ///
    /// <returns>True if the window is active; otherwise, false.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool IsWindowActive(PersistedMaintenanceState state)
        => state.IsEnabled && (state.StartsAt is not { } startsAt || startsAt <= DateTimeOffset.UtcNow);

    /// <summary>
    /// Checks whether a request matches an allowed path, prefix, or IP address.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="state">The current maintenance window.</param>
    ///
    /// <returns>True if the request may pass through; otherwise, false.</returns>
    ///
    /// <remarks>
    /// Exact paths match only that path. Prefixes match on path-segment boundaries.
    /// Both comparisons are case-insensitive.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool ShouldPassThrough(HttpContext context, PersistedMaintenanceState state)
    {
        PathString path = context.Request.Path;

        IReadOnlyList<string> paths = state.AllowedPaths ?? [];

        if (paths.Any(allowed => path.Equals(allowed, StringComparison.OrdinalIgnoreCase)))
            return true;

        paths = state.AllowedPathPrefixes ?? [];

        return paths.Any(prefix => path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
               || IsAllowedAddress(context, state);
    }

    /// <summary>
    /// Checks whether the connection's remote IP address is allowed.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="state">The current maintenance window.</param>
    ///
    /// <returns>True if the remote address is allowed; otherwise, false.</returns>
    ///
    /// <remarks>
    /// Behind a reverse proxy, configure trusted forwarded headers before this middleware.
    /// Entries must be individual IP addresses; invalid entries are ignored.
    /// IPv4-mapped IPv6 addresses are normalized on both sides before comparison.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool IsAllowedAddress(HttpContext context, PersistedMaintenanceState state)
    {
        if (state.AllowedIpAddresses is not { Count: > 0 } allowed || context.Connection.RemoteIpAddress is not { } remote)
            return false;

        IPAddress candidate = remote.IsIPv4MappedToIPv6 ? remote.MapToIPv4() : remote;

        foreach (string entry in allowed)
        {
            if (!IPAddress.TryParse(entry, out IPAddress? parsed))
                continue;

            if (parsed.IsIPv4MappedToIPv6)
                parsed = parsed.MapToIPv4();

            if (parsed.Equals(candidate))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks whether the request explicitly accepts HTML.
    /// </summary>
    ///
    /// <param name="request">The current HTTP request.</param>
    ///
    /// <returns>
    /// True if the Accept header includes text/html with a positive quality value;
    /// otherwise, false.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool AcceptsHtml(HttpRequest request) => request.GetTypedHeaders().Accept
        .Where(accept => accept.Quality.GetValueOrDefault(1) > 0)
        .Any(accept => accept.MediaType.HasValue && accept.MediaType.Value.Equals("text/html", StringComparison.OrdinalIgnoreCase));
}
