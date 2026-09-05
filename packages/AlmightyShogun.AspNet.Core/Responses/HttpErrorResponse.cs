namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The error body every failure in the application serializes to, so a client can parse one shape regardless of whether
/// the failure came from an exception, a filter, or the pipeline below MVC.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public record HttpErrorResponse
{
    /// <summary>
    /// The status code, repeated in the body so a client that has lost the response headers, as through a logging
    /// or proxy layer, can still tell what happened.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required int Code { get; init; }

    /// <summary>
    /// The machine-readable identifier to branch on, such as <c>not_found</c> or a code an
    /// <see cref="ErrorMapping"/> supplied. This is the field a client should switch on, never the description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Error { get; init; }

    /// <summary>
    /// The human-readable description as whoever wrote the error supplied it, or <c>null</c> when the error was
    /// written without one. Nothing here resolves or formats it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? ErrorDescription { get; init; }
}
