namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Configures how handled errors are written and logged. Bound from the optional <c>HttpErrors</c> configuration
/// section; every value has a default, so the section may be absent entirely.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record HttpErrorSettings
{
    /// <summary>
    /// Gets whether error bodies are written as <c>application/problem+json</c> following RFC 9457 instead of the
    /// package shape. Disabled by default, because turning it on changes every error body the application returns.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool UseProblemDetails { get; init; }

    /// <summary>
    /// Gets whether handled errors are logged at all. Enabled by default, because an error response with no log line
    /// leaves nothing to diagnose from.
    /// </summary>
    ///
    /// <remarks>
    /// Applies only to <see cref="AppExceptionHandler"/>. An unhandled exception is still logged by the framework's own
    /// handler middleware, so turning this off silences deliberate application errors and not genuine faults.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool LogErrors { get; init; } = true;

    /// <summary>
    /// Gets the lowest status code that is logged. Compared against <see cref="IAppException.StatusCode"/>, so an error
    /// below it is answered normally but recorded nowhere. Raise it to <c>500</c> to keep client faults out of the log.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int MinimumLogStatusCode { get; init; } = 400;
}
