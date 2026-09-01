using System.Net;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Handles ASP.NET Core requests by enforcing the current maintenance mode state.
/// </summary>
///
/// <param name="next">The next request delegate in the pipeline.</param>
/// <param name="maintenanceService">The service used to read the current maintenance state.</param>
/// <param name="maintenanceOptions">The bound maintenance settings.</param>
/// <param name="responseWriter">The shared writer used for the blocked-request error body.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class MaintenanceMiddleware(
    RequestDelegate next,
    MaintenanceService maintenanceService,
    IOptions<MaintenanceSettings> maintenanceOptions,
    IHttpErrorResponseWriter responseWriter
)
{
    /// <summary>
    /// The maintenance path, normalized once rather than per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PathString _maintenancePath = MaintenancePath.Normalize(maintenanceOptions.Value.MaintenancePath, "/maintenance");

    /// <summary>
    /// Handles the request and either passes it through or returns the maintenance response.
    /// </summary>
    ///
    /// <param name="context">The request being considered, read for its path, its accepted media types, and its connecting address.</param>
    ///
    /// <returns>A task that completes when the request has been handled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
            context.Response.Redirect(_maintenancePath);

            return;
        }

        await responseWriter.WriteAsync(
            context,
            StatusCodes.Status503ServiceUnavailable,
            "service_unavailable",
            state.Message,
            context.RequestAborted
        );
    }

    /// <summary>
    /// Answers the maintenance path itself with the current window, so a blocked visitor is told what is happening rather than only
    /// refused.
    /// </summary>
    ///
    /// <param name="context">The request whose response the details are written to.</param>
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns>A task that completes when the response has been written.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task WriteDetailsAsync(HttpContext context, PersistedMaintenanceState state)
    {
        SetRetryAfter(context, state);

        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        await context.Response.WriteAsJsonAsync(
            new MaintenanceResponse(state.Message, state.StartsAt, state.EndsAt, state.EnabledAt),
            context.RequestAborted
        );
    }

    /// <summary>
    /// Writes the <c>Retry-After</c> header when the end time is known.
    /// </summary>
    ///
    /// <param name="context">The request whose response header is set, and which is left alone once it has started.</param>
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <remarks>
    /// Only written when <c>EndsAt</c> is set. Inventing a duration that is not known would be worse than omitting it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void SetRetryAfter(HttpContext context, PersistedMaintenanceState state)
    {
        if (state.EndsAt is not { } endsAt || context.Response.HasStarted) return;

        double seconds = Math.Ceiling((endsAt - DateTimeOffset.UtcNow).TotalSeconds);

        if (seconds > 0)
            context.Response.Headers.RetryAfter = seconds.ToString("F0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Determines whether the maintenance window is currently in force.
    /// </summary>
    ///
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns><c>true</c> when maintenance is enabled and the window has started.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsWindowActive(PersistedMaintenanceState state)
        => state.IsEnabled && (state.StartsAt is not { } startsAt || startsAt <= DateTimeOffset.UtcNow);

    /// <summary>
    /// Determines whether a request stays available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <param name="context">
    /// The request being considered, read for its path and, through the address check, its connecting address.
    /// </param>
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns><c>true</c> when the request should be served normally.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Reports whether the connecting address is on the allow list. Read from the connection rather than a forwarded header, so a client
    /// cannot let itself through by claiming an address.
    /// </summary>
    ///
    /// <param name="context">The request being considered, read for its connecting address alone.</param>
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns><c>true</c> when the address is allowed.</returns>
    ///
    /// <remarks>
    /// Read from the connection, never from a header. A header-derived address is forgeable by the caller, which would make this bypass
    /// worse than having none.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAllowedAddress(HttpContext context, PersistedMaintenanceState state)
    {
        if (state.AllowedIpAddresses is not { Count: > 0 } allowed || context.Connection.RemoteIpAddress is not { } remote)
            return false;

        IPAddress candidate = remote.IsIPv4MappedToIPv6 ? remote.MapToIPv4() : remote;

        foreach (string entry in allowed)
            if (IPAddress.TryParse(entry, out IPAddress? parsed) && parsed.Equals(candidate))
                return true;

        return false;
    }

    /// <summary>
    /// Reports whether the caller is a browser rather than an API client, which is what decides between redirecting to the details page and
    /// returning the error body.
    /// </summary>
    ///
    /// <param name="request">The current request.</param>
    ///
    /// <returns>
    /// <c>true</c> when an <c>Accept</c> entry names <c>text/html</c> and does not refuse it with <c>q=0</c>. A client sending only
    /// <c>*/*</c> reads as an API client, since the wildcard is not expanded.
    /// </returns>
    ///
    /// <remarks>
    /// Deciding on the header rather than on configuration is what lets one window serve a browser the maintenance page and an API client
    /// the error body. Whether a redirect happens at all is still governed by <c>RedirectBlockedRequests</c>.
    /// </remarks>
    ///
    /// <remarks>
    /// The header is parsed rather than searched for a substring, because <c>text/html;q=0</c> contains the media type while explicitly
    /// refusing it, and a client that refuses HTML should be answered with the error body rather than redirected to a page.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool AcceptsHtml(HttpRequest request) => request.GetTypedHeaders().Accept
        .Any(accept => accept.Quality.GetValueOrDefault(1) > 0
                       && accept.MediaType.HasValue
                       && accept.MediaType.Value.Equals("text/html", StringComparison.OrdinalIgnoreCase));
}
