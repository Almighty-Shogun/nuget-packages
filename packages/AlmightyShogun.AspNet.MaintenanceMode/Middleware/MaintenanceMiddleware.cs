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
/// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private readonly PathString _maintenancePath = MaintenancePath.Normalize(maintenanceOptions.Value.MaintenancePath, "/maintenance");

    /// <summary>
    /// Handles the request, either passing it on or answering it from the current window.
    /// </summary>
    ///
    /// <param name="context">The request being considered, read for its path, its accepted media types, and its connecting address.</param>
    ///
    /// <returns>A task that completes when the request has been handled.</returns>
    ///
    /// <exception cref="IOException">
    /// An expired window that lifts itself was being closed and its file could not be deleted. Nothing here catches it, so an ordinary
    /// request fails rather than being served or blocked.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not delete the state file of an expired window, which fails the request in the same way.
    /// </exception>
    ///
    /// <remarks>
    /// The maintenance path is claimed before the window is checked and before every allow list, and the rest of the pipeline never runs
    /// for it: with a window in force it answers <c>503</c> and the window as JSON, and with none it answers <c>404</c>. An application
    /// route at that path is therefore unreachable, and a status page polling it is answered <c>404</c> while the site is up.
    /// </remarks>
    ///
    /// <remarks>
    /// Any other request is passed on when no window is in force or <see cref="ShouldPassThrough"/> lets it through, and is otherwise
    /// blocked: redirected to the maintenance path when the window redirects and the caller accepts HTML, and given the <c>503</c> error
    /// body otherwise.
    /// </remarks>
    ///
    /// <remarks>
    /// The configured maintenance path is relative to the path base in both places it is used: the match above is against
    /// <c>Request.Path</c> and the redirect prefixes the request's own <c>PathBase</c>, so an application mounted under a base sends the
    /// browser somewhere inside itself rather than to the host root. Both read the request as it stands here, so whatever establishes the
    /// base has to run first: ahead of <c>UsePathBase</c> the prefix is still part of <c>Request.Path</c> and no configured maintenance
    /// path matches it, and ahead of the forwarded-headers middleware the base is empty and the redirect leaves the application.
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
    /// Only written when <c>EndsAt</c> is set, and only while it is still in the future: a window that has outlived its estimate carries no
    /// header at all.
    /// </remarks>
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
    /// Determines whether the maintenance window is currently in force.
    /// </summary>
    ///
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns><c>true</c> when maintenance is enabled and the window has started.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <remarks>
    /// An allowed path is compared against the whole request path, ordinal and case-insensitive, so it opens that one path and nothing
    /// beneath it. An allowed prefix is matched on segment boundaries instead, so <c>/api</c> opens <c>/api/orders</c> but not
    /// <c>/apixyz</c>. Both lists are consulted before the address allow list.
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
    /// Reports whether the connecting address is on the allow list, read from the connection so a caller cannot put itself on that list
    /// by sending a header.
    /// </summary>
    ///
    /// <param name="context">The request being considered, read for its connecting address alone.</param>
    /// <param name="state">The window as it currently stands, with configured defaults and the expiry policy already applied.</param>
    ///
    /// <returns><c>true</c> when the address is allowed.</returns>
    ///
    /// <remarks>
    /// Read from the connection, never from a header of this middleware's own reading. A header-derived address is forgeable by the
    /// caller, which would make this bypass worse than having none.
    /// </remarks>
    ///
    /// <remarks>
    /// That only holds where the connection address is the caller's. Behind a reverse proxy it is the proxy's until
    /// <c>UseForwardedHeaders</c> has rewritten it, so an application behind one has to run that first and declare its trusted proxies;
    /// otherwise every caller arrives as the proxy and the allow list either matches all of them or none.
    /// </remarks>
    ///
    /// <remarks>
    /// An entry opens one address and nothing else: it is parsed as a single <see cref="IPAddress"/> and compared for equality, so CIDR
    /// notation and ranges never match. An entry that fails to parse is skipped in silence, with no log and no startup validation, so a
    /// mistyped address is a bypass that never fires and says nothing about it.
    /// </remarks>
    ///
    /// <remarks>
    /// A connection address arriving IPv4-mapped over a dual-stack socket is folded down to IPv4 first, which is what lets a plain
    /// <c>127.0.0.1</c> entry match it. That folding is applied to the connection address only, never to the configured entry.
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
    /// <since>4.0.0</since>
    private static bool AcceptsHtml(HttpRequest request) => request.GetTypedHeaders().Accept
        .Any(accept => accept.Quality.GetValueOrDefault(1) > 0
                       && accept.MediaType.HasValue
                       && accept.MediaType.Value.Equals("text/html", StringComparison.OrdinalIgnoreCase));
}
