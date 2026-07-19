using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Handles ASP.NET Core requests by enforcing the current maintenance mode state.
/// </summary>
///
/// <param name="next">The next request delegate in the pipeline.</param>
/// <param name="maintenanceService">The service used to read the current maintenance state.</param>
/// <param name="maintenanceOptions">The bound maintenance settings.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class MaintenanceMiddleware(RequestDelegate next, IMaintenanceService maintenanceService, IOptions<MaintenanceSettings> maintenanceOptions)
{
    /// <summary>
    /// Stores the bound maintenance settings used while handling requests.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly MaintenanceSettings _maintenanceSettings = maintenanceOptions.Value;

    /// <summary>
    /// Handles the request and either passes it through or returns the maintenance response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task that completes when the request has been handled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task InvokeAsync(HttpContext context)
    {
        MaintenanceState state = await maintenanceService.GetAsync();
        string maintenancePath = NormalizePath(_maintenanceSettings.MaintenancePath, "/maintenance");

        if (IsMaintenancePath(context.Request.Path, maintenancePath))
        {
            if (!state.IsEnabled)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await WriteMaintenanceDetailsAsync(context, state);

            return;
        }

        if (ShouldPassThrough(context.Request.Path, state))
        {
            await next(context);

            return;
        }

        if (state.RedirectBlockedRequests)
        {
            context.Response.Redirect(maintenancePath);

            return;
        }

        await WriteMaintenanceDetailsAsync(context, state);
    }

    /// <summary>
    /// Writes the JSON maintenance response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="state">The current maintenance state.</param>
    ///
    /// <returns>A task that completes when the response has been written.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task WriteMaintenanceDetailsAsync(HttpContext context, MaintenanceState state)
    {
        if (state.EndsAt is not null)
        {
            TimeSpan retryAfter = state.EndsAt.Value - DateTime.UtcNow;

            if (retryAfter > TimeSpan.Zero)
                context.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString("F0");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        await context.Response.WriteAsJsonAsync(new
        {
            state.Message,
            state.EndsAt,
            state.EnabledAt
        }, context.RequestAborted);
    }

    /// <summary>
    /// Determines whether the request should continue through the pipeline.
    /// </summary>
    ///
    /// <param name="path">The request path.</param>
    /// <param name="state">The current maintenance state.</param>
    ///
    /// <returns><c>true</c> when maintenance is disabled or the path is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ShouldPassThrough(PathString path, MaintenanceState state)
        => !state.IsEnabled || IsAllowed(path, state);

    /// <summary>
    /// Checks whether a request path is allowed by the current maintenance state.
    /// </summary>
    ///
    /// <param name="path">The request path.</param>
    /// <param name="state">The current maintenance state.</param>
    ///
    /// <returns><c>true</c> when the path is explicitly allowed or matches an allowed prefix; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAllowed(PathString path, MaintenanceState state)
    {
        string requestPath = NormalizePath(path.Value, "/");

        return IsAllowedPath(requestPath, state.AllowedPaths)
               || IsAllowedPathPrefix(requestPath, state.AllowedPathPrefixes);
    }

    /// <summary>
    /// Checks whether a request path is the configured maintenance details path.
    /// </summary>
    ///
    /// <param name="path">The request path.</param>
    /// <param name="maintenancePath">The normalized maintenance details path.</param>
    ///
    /// <returns><c>true</c> when the request path matches the maintenance details path; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsMaintenancePath(PathString path, string maintenancePath)
        => string.Equals(NormalizePath(path.Value, "/"), maintenancePath, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a request path matches a configured path prefix.
    /// </summary>
    ///
    /// <param name="requestPath">The normalized request path.</param>
    /// <param name="prefix">The normalized allowed prefix.</param>
    ///
    /// <returns><c>true</c> when the request path matches the prefix exactly or by child path; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool MatchesPrefix(string requestPath, string prefix)
        => requestPath.Equals(prefix, StringComparison.OrdinalIgnoreCase)
           || requestPath.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a request path is included in the allowed path list.
    /// </summary>
    ///
    /// <param name="requestPath">The normalized request path.</param>
    /// <param name="allowedPaths">The configured allowed paths.</param>
    ///
    /// <returns><c>true</c> when the request path is explicitly allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAllowedPath(string requestPath, IReadOnlyList<string> allowedPaths)
        => allowedPaths.Any(allowedPath => !string.IsNullOrWhiteSpace(allowedPath)
                                           && string.Equals(NormalizePath(allowedPath, "/"), requestPath, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Checks whether a request path matches any allowed path prefix.
    /// </summary>
    ///
    /// <param name="requestPath">The normalized request path.</param>
    /// <param name="allowedPathPrefixes">The configured allowed path prefixes.</param>
    ///
    /// <returns><c>true</c> when the request path matches an allowed prefix; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAllowedPathPrefix(string requestPath, IReadOnlyList<string> allowedPathPrefixes)
        => allowedPathPrefixes.Any(prefix => !string.IsNullOrWhiteSpace(prefix)
                                             && MatchesPrefix(requestPath, NormalizePath(prefix, "/")));

    /// <summary>
    /// Normalizes a path so it is trimmed, starts with a slash, and does not end with a slash unless it is the root path.
    /// </summary>
    ///
    /// <param name="path">The path to normalize.</param>
    /// <param name="fallback">The path to use when <paramref name="path"/> is null, empty, or whitespace.</param>
    ///
    /// <returns>The normalized path.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string NormalizePath(string? path, string fallback)
    {
        if (string.IsNullOrWhiteSpace(path))
            return fallback;

        string normalized = path.Trim();
        normalized = normalized.StartsWith('/') ? normalized : $"/{normalized}";
        normalized = normalized.TrimEnd('/');

        return string.IsNullOrEmpty(normalized) ? "/" : normalized;
    }
}
